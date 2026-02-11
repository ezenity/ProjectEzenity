using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Ezenity.DTOs.Models.Files;
using Ezenity.API.Options;
using Ezenity.Core.Services.Files;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ezenity.Infrastructure.Services.Files
{
    /// <summary>
    /// Local filesystem storage with JSON sidecar metadata:
    /// - File bytes stored as: {id}{ext}
    /// - Metadata stored as:  {id}.meta.json
    /// </summary>
    public sealed class LocalFileStorageService : IFileStorageService
    {
        private static readonly Regex IdRegex = new("^[a-f0-9]{32}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly FileStorageOptions _options;
        private readonly string _rootFullPath;
        private readonly HashSet<string> _allowedExtensions;
        private readonly FileExtensionContentTypeProvider _contentTypes;
        private readonly ILogger<LocalFileStorageService> _logger;

        public LocalFileStorageService(
            IOptions<FileStorageOptions> options,
            IWebHostEnvironment env,
            FileExtensionContentTypeProvider contentTypes,
            ILogger<LocalFileStorageService> logger)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _contentTypes = contentTypes ?? throw new ArgumentNullException(nameof(contentTypes));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _allowedExtensions = new HashSet<string>(
                (_options.AllowedExtensions ?? new List<string>())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim().StartsWith(".") ? x.Trim() : "." + x.Trim()),
                StringComparer.OrdinalIgnoreCase);

            var root = string.IsNullOrWhiteSpace(_options.RootPath) ? "./files" : _options.RootPath.Trim();

            // Resolve relative root against content root
            _rootFullPath = Path.IsPathRooted(root)
                ? Path.GetFullPath(root)
                : Path.GetFullPath(Path.Combine(env.ContentRootPath, root));

            Directory.CreateDirectory(_rootFullPath);

            _logger.LogInformation("FileStorage RootPath resolved to: {Root}", _rootFullPath);
        }

        public async Task<FileItemResponse> SaveAsync(IFormFile file, string? scope, CancellationToken ct)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (file.Length <= 0) throw new InvalidOperationException("No file uploaded.");
            if (file.Length > _options.MaxUploadBytes)
                throw new InvalidOperationException($"File too large. Max allowed is {_options.MaxUploadBytes} bytes.");

            var ext = Path.GetExtension(file.FileName) ?? string.Empty;
            if (string.IsNullOrWhiteSpace(ext))
                throw new InvalidOperationException("File extension is missing.");

            ext = ext.Trim();
            if (!_allowedExtensions.Contains(ext))
                throw new InvalidOperationException($"File type '{ext}' not allowed.");

            var id = Guid.NewGuid().ToString("N");
            var storedName = $"{id}{ext}";
            var storedPath = CombineUnderRoot(storedName);

            await using (var stream = new FileStream(storedPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 64 * 1024, useAsync: true))
            {
                await file.CopyToAsync(stream, ct);
            }

            if (!_contentTypes.TryGetContentType(storedPath, out var contentType))
                contentType = file.ContentType ?? "application/octet-stream";

            var meta = new FileItemResponse
            {
                Id = id,
                OriginalName = Path.GetFileName(file.FileName),
                ContentType = contentType,
                Size = file.Length,
                CreatedUtc = DateTime.UtcNow,
                Scope = string.IsNullOrWhiteSpace(scope) ? null : scope.Trim(),
                Url = null // controller fills
            };

            var metaPath = CombineUnderRoot($"{id}.meta.json");
            var json = JsonSerializer.Serialize(meta, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(metaPath, json, ct);

            return meta;
        }

        public async Task<(Stream Stream, FileItemResponse Meta)> OpenReadAsync(string fileId, CancellationToken ct)
        {
            var meta = await GetMetaRequiredAsync(fileId, ct);
            var storedPath = ResolveStoredPath(meta.Id);

            // For videos, enableRangeProcessing happens in controller via File(stream, contentType, enableRangeProcessing:true)
            var stream = new FileStream(storedPath, FileMode.Open, FileAccess.Read, FileShare.Read, 64 * 1024, useAsync: true);
            return (stream, meta);
        }

        public async Task<FileItemResponse?> GetMetaAsync(string fileId, CancellationToken ct)
        {
            if (!IsValidId(fileId)) return null;

            var metaPath = CombineUnderRoot($"{fileId}.meta.json");
            if (!File.Exists(metaPath)) return null;

            var json = await File.ReadAllTextAsync(metaPath, ct);
            var meta = JsonSerializer.Deserialize<FileItemResponse>(json);

            return meta;
        }

        public async Task<bool> DeleteAsync(string fileId, CancellationToken ct)
        {
            if (!IsValidId(fileId)) return false;

            var meta = await GetMetaAsync(fileId, ct);
            if (meta == null) return false;

            var metaPath = CombineUnderRoot($"{fileId}.meta.json");
            var storedPath = ResolveStoredPath(fileId);

            try
            {
                if (File.Exists(storedPath)) File.Delete(storedPath);
                if (File.Exists(metaPath)) File.Delete(metaPath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed deleting fileId={FileId}", fileId);
                return false;
            }
        }

        public async Task<IReadOnlyList<FileItemResponse>> ListAsync(string? scope, CancellationToken ct)
        {
            var normalizedScope = string.IsNullOrWhiteSpace(scope) ? null : scope.Trim();

            var metas = Directory.EnumerateFiles(_rootFullPath, "*.meta.json", SearchOption.TopDirectoryOnly);

            var results = new List<FileItemResponse>();

            foreach (var metaPath in metas)
            {
                ct.ThrowIfCancellationRequested();

                try
                {
                    var json = await File.ReadAllTextAsync(metaPath, ct);
                    var meta = JsonSerializer.Deserialize<FileItemResponse>(json);
                    if (meta == null) continue;

                    if (normalizedScope != null && !string.Equals(meta.Scope, normalizedScope, StringComparison.OrdinalIgnoreCase))
                        continue;

                    results.Add(meta);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Skipping invalid meta file: {MetaPath}", metaPath);
                }
            }

            return results
                .OrderByDescending(x => x.CreatedUtc)
                .ToList();
        }

        private async Task<FileItemResponse> GetMetaRequiredAsync(string fileId, CancellationToken ct)
        {
            var meta = await GetMetaAsync(fileId, ct);
            if (meta == null)
                throw new FileNotFoundException("File metadata not found.");

            return meta;
        }

        private bool IsValidId(string fileId) =>
            !string.IsNullOrWhiteSpace(fileId) && IdRegex.IsMatch(fileId);

        private string ResolveStoredPath(string fileId)
        {
            // Find stored file by known allowed extensions based on the id.
            // This avoids trusting user input for a filename.
            foreach (var ext in _allowedExtensions)
            {
                var candidate = CombineUnderRoot($"{fileId}{ext}");
                if (File.Exists(candidate))
                    return candidate;
            }

            throw new FileNotFoundException("Stored file not found.");
        }

        private string CombineUnderRoot(string fileName)
        {
            // Prevent path traversal: only allow a file name (no directories)
            var safeName = Path.GetFileName(fileName);
            var combined = Path.GetFullPath(Path.Combine(_rootFullPath, safeName));

            if (!combined.StartsWith(_rootFullPath, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Invalid path resolution.");

            return combined;
        }
    }
}
