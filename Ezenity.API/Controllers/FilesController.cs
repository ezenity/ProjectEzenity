using Asp.Versioning;
using Ezenity.DTOs;
using Ezenity.DTOs.Models;
using Ezenity.DTOs.Models.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ezenity.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/files")]
    [ApiVersion("1.0")]
    [Produces("application/vnd.api+json", "application/json")]
    public class FilesController : ControllerBase
    {
        private readonly FileExtensionContentTypeProvider _contentTypes;
        private readonly string _root;

        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".webp", ".gif",
            ".mp4", ".webm", ".mov"
        };

        public FilesController(
            FileExtensionContentTypeProvider contentTypes,
            IConfiguration config)
        {
            _contentTypes = contentTypes ?? throw new ArgumentNullException(nameof(contentTypes));

            _root = config["FileStorage:RootPath"] ?? "./files";
            Directory.CreateDirectory(_root);
        }

        // GET /api/v1/files/list?scope=vault
        [HttpGet("list")]
        public async Task<ActionResult<ApiResponse<IEnumerable<FileItemResponse>>>> ListFilesAsync([FromQuery] string? scope = null)
        {
            var metas = Directory.EnumerateFiles(_root, "*.meta.json", SearchOption.TopDirectoryOnly);

            var items = new List<FileItemResponse>();

            foreach (var metaPath in metas)
            {
                try
                {
                    var json = await System.IO.File.ReadAllTextAsync(metaPath);
                    var item = JsonSerializer.Deserialize<FileItemResponse>(json);

                    if (item == null) continue;

                    if (!string.IsNullOrWhiteSpace(scope) &&
                        !string.Equals(item.Scope, scope, StringComparison.OrdinalIgnoreCase))
                        continue;

                    // Ensure URL is always correct for this host
                    item.Url = Url.Action(nameof(GetFileAsync), new { version = "1.0", fileId = item.Id })!;
                    items.Add(item);
                }
                catch
                {
                    // ignore broken meta files
                }
            }

            return Ok(new ApiResponse<IEnumerable<FileItemResponse>>
            {
                StatusCode = 200,
                IsSuccess = true,
                Message = "Files fetched successfully.",
                Data = items.OrderByDescending(x => x.CreatedUtc).ToList()
            });
        }

        // GET /api/v1/files/{fileId}
        [HttpGet("{fileId}")]
        public async Task<IActionResult> GetFileAsync(string fileId)
        {
            if (!Guid.TryParseExact(fileId, "N", out var id))
                return NotFound();

            var asset = await _dataContext.FileAssets.FirstOrDefaultAsync(x => x.Id == id);
            if (asset == null)
                return NotFound();

            var fullPath = Path.Combine(_root, asset.StoredName);
            if (!System.IO.File.Exists(fullPath))
                return NotFound();

            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);

            // enableRangeProcessing is important for video playback
            return File(stream, asset.ContentType, fileDownloadName: asset.OriginalName, enableRangeProcessing: true);
        }

        // POST /api/v1/files/upload (multipart/form-data)
        [HttpPost("upload", Name = "UploadFile")]
        [RequestSizeLimit(50_000_000)] // 50MB (adjust)
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<UploadFileResponse>>> UploadAsync(
            [FromForm] IFormFile file,
            [FromForm] string? scope = null)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new ApiResponse<object> { StatusCode = 400, IsSuccess = false, Message = "No file uploaded." });

            var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(ext) || !AllowedExtensions.Contains(ext))
                return BadRequest(new ApiResponse<object> { StatusCode = 400, IsSuccess = false, Message = $"File type '{ext}' not allowed." });

            Directory.CreateDirectory(_root);

            var id = Guid.NewGuid();
            var storedName = $"{id:N}{ext}";
            var storedPath = Path.Combine(_root, storedName);

            await using (var stream = new FileStream(storedPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                await file.CopyToAsync(stream);
            }

            if (!_contentTypes.TryGetContentType(storedPath, out var contentType))
                contentType = file.ContentType ?? "application/octet-stream";

            // TODO: resolve current user from JWT if you want ownership
            int? createdByAccountId = null;

            var asset = new FileAsset
            {
                Id = id,
                OriginalName = Path.GetFileName(file.FileName),
                StoredName = storedName,
                ContentType = contentType,
                Size = file.Length,
                Scope = string.IsNullOrWhiteSpace(scope) ? null : scope.Trim(),
                CreatedUtc = DateTime.UtcNow,
                CreatedByAccountId = createdByAccountId
            };

            _dataContext.FileAssets.Add(asset);
            await _dataContext.SaveChangesAsync();

            var item = new FileItemResponse
            {
                Id = asset.Id.ToString("N"),
                OriginalName = asset.OriginalName,
                ContentType = asset.ContentType,
                Size = asset.Size,
                CreatedUtc = asset.CreatedUtc,
                Scope = asset.Scope,
                Url = Url.Action(nameof(GetFileAsync), new { version = "1.0", fileId = asset.Id.ToString("N") })!
            };

            return Ok(new ApiResponse<UploadFileResponse>
            {
                StatusCode = 200,
                IsSuccess = true,
                Message = "File uploaded successfully.",
                Data = new UploadFileResponse { File = item }
            });
        }

        // DELETE /api/v1/files/{fileId}
        [HttpDelete("{fileId}")]
        public IActionResult DeleteFileAsync(string fileId)
        {
            var filePath = FindStoredFilePath(fileId);
            if (filePath == null) return NotFound(new { message = "File not found." });

            var metaPath = Path.Combine(_root, $"{fileId}.meta.json");

            System.IO.File.Delete(filePath);
            if (System.IO.File.Exists(metaPath)) System.IO.File.Delete(metaPath);

            return Ok(new { message = "File deleted successfully." });
        }

        private string? FindStoredFilePath(string fileId)
        {
            // file stored as {id}.{ext}
            var matches = Directory.GetFiles(_root, $"{fileId}.*", SearchOption.TopDirectoryOnly)
                                   .Where(p => !p.EndsWith(".meta.json", StringComparison.OrdinalIgnoreCase))
                                   .ToList();

            return matches.FirstOrDefault();
        }
    }
}
