using Asp.Versioning;
using Ezenity.API.Options;
using Ezenity.Core.Services.Files;
using Ezenity.DTOs.Models;
using Ezenity.DTOs.Models.Files;
using Ezenity.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly DataContext _dataContext;
        private readonly IFileStorageService _storage;
        private readonly FileStorageOptions _options;

        public FilesController(
            FileExtensionContentTypeProvider contentTypes,
            DataContext dataContext,
            IFileStorageService storage,
            IOptions<FileStorageOptions> options)
        {
            _contentTypes = contentTypes ?? throw new ArgumentNullException(nameof(contentTypes));
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        // GET /api/v1/files/list?scope=vault
        [HttpGet("list")]
        public async Task<ActionResult<ApiResponse<IEnumerable<FileItemResponse>>>> ListFilesAsync([FromQuery] string? scope = null)
        {
            var query = _dataContext.FileAssets.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(scope))
            {
                var trimmed = scope.Trim();
                query = query.Where(x => x.Scope != null && x.Scope == trimmed);
            }

            var assets = await query
                .OrderByDescending(x => x.CreatedUtc)
                .ToListAsync();

            var items = assets.Select(asset => new FileItemResponse
            {
                Id = asset.Id.ToString("N"),
                OriginalName = asset.OriginalName,
                ContentType = asset.ContentType,
                Size = asset.Size,
                CreatedUtc = asset.CreatedUtc,
                Scope = asset.Scope,
                Url = Url.Action(nameof(GetFileAsync), new { version = "1.0", fileId = asset.Id.ToString("N") })!
            }).ToList();

            return Ok(new ApiResponse<IEnumerable<FileItemResponse>>
            {
                StatusCode = 200,
                IsSuccess = true,
                Message = "Files fetched successfully.",
                Data = items
            });
        }

        // GET /api/v1/files/{fileId}
        [HttpGet("{fileId}")]
        public async Task<IActionResult> GetFileAsync(string fileId)
        {
            if (!Guid.TryParseExact(fileId, "N", out var id))
                return NotFound(new ApiResponse<object> { StatusCode = 404, IsSuccess = false, Message = "File not found." });

            var asset = await _dataContext.FileAssets.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (asset == null)
                return NotFound(new ApiResponse<object> { StatusCode = 404, IsSuccess = false, Message = "File not found." });

            var exists = await _storage.ExistsAsync(asset.StoredName);
            if (!exists)
                return NotFound(new ApiResponse<object> { StatusCode = 404, IsSuccess = false, Message = "File bytes missing on disk." });

            var stream = await _storage.OpenReadAsync(asset.StoredName);

            var contentType = asset.ContentType;
            if (string.IsNullOrWhiteSpace(contentType))
            {
                if (!_contentTypes.TryGetContentType(asset.StoredName, out contentType))
                    contentType = "application/octet-stream";
            }

            return File(stream, contentType, fileDownloadName: asset.OriginalName, enableRangeProcessing: true);
        }

        // POST /api/v1/files/upload (multipart/form-data)
        [HttpPost("upload", Name = "UploadFile")]
        [RequestSizeLimit(50_000_000)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<UploadFileResponse>>> UploadAsync(
            [FromForm] IFormFile file,
            [FromForm] string? scope = null)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new ApiResponse<object> { StatusCode = 400, IsSuccess = false, Message = "No file uploaded." });

            if (_options.MaxUploadBytes > 0 && file.Length > _options.MaxUploadBytes)
                return BadRequest(new ApiResponse<object> { StatusCode = 400, IsSuccess = false, Message = $"File too large. Max allowed is {_options.MaxUploadBytes} bytes." });

            var ext = Path.GetExtension(file.FileName)?.Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(ext) || _options.AllowedExtensions == null || !_options.AllowedExtensions.Any(x => string.Equals(x, ext, StringComparison.OrdinalIgnoreCase)))
                return BadRequest(new ApiResponse<object> { StatusCode = 400, IsSuccess = false, Message = $"File type '{ext}' not allowed." });

            var id = Guid.NewGuid();

            string storedName;
            await using (var input = file.OpenReadStream())
            {
                storedName = await _storage.SaveAsync(id, ext, input);
            }

            var contentType = file.ContentType;
            if (string.IsNullOrWhiteSpace(contentType))
            {
                if (!_contentTypes.TryGetContentType(storedName, out contentType))
                    contentType = "application/octet-stream";
            }

            // TODO: if you want ownership, resolve from JWT and set CreatedByAccountId
            int? createdByAccountId = null;

            var asset = new Ezenity.Core.Entities.Files.FileAsset
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
        public async Task<ActionResult<ApiResponse<object>>> DeleteFileAsync(string fileId)
        {
            if (!Guid.TryParseExact(fileId, "N", out var id))
                return NotFound(new ApiResponse<object> { StatusCode = 404, IsSuccess = false, Message = "File not found." });

            var asset = await _dataContext.FileAssets.FirstOrDefaultAsync(x => x.Id == id);
            if (asset == null)
                return NotFound(new ApiResponse<object> { StatusCode = 404, IsSuccess = false, Message = "File not found." });

            await _storage.DeleteAsync(asset.StoredName);

            _dataContext.FileAssets.Remove(asset);
            await _dataContext.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                StatusCode = 200,
                IsSuccess = true,
                Message = "File deleted successfully.",
                Data = null
            });
        }
    }
}
