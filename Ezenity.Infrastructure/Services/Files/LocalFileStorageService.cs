using System.Text.RegularExpressions;
using Ezenity.Domain.Options;
using Ezenity.Domain.Entities.Files;
using Ezenity.Infrastructure.Data;
using Ezenity.Application.Abstractions.Files;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ezenity.Infrastructure.Services.Files;

/// <summary>
/// Local filesystem storage for file bytes + MySQL metadata via FileAssets table.
///
/// - File bytes stored as: {id}{ext} under RootPath
/// - Metadata stored in DB: FileAsset
///
/// Controller is responsible for setting Url on FileItemResponse.
/// </summary>
public sealed class LocalFileStorageService : IFileStorageService
{
    private static readonly Regex IdRegex = new("^[a-f0-9]{32}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly DataContext _db;
    private readonly FileStorageOptions _options;
    private readonly string _rootFullPath;
    private readonly HashSet<string> _allowedExtensions;
    private readonly FileExtensionContentTypeProvider _contentTypes;
    private readonly ILogger<LocalFileStorageService> _logger;

    public LocalFileStorageService(
        DataContext db,
        IOptions<FileStorageOptions> options,
        IWebHostEnvironment env,
        FileExtensionContentTypeProvider contentTypes,
        IConfiguration config,
        ILogger<LocalFileStorageService> logger)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _contentTypes = contentTypes ?? throw new ArgumentNullException(nameof(contentTypes));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

        // Optional env override in your "EZENITY_*" style.
        // This means you can keep appsettings clean and drive it purely from docker-compose env vars.
        var envRoot = config["EZENITY_FILES_ROOT"];
        if (!string.IsNullOrWhiteSpace(envRoot))
            _options.RootPath = envRoot.Trim();

        var allowed = (_options.AllowedExtensions ?? new List<string>())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x =>
            {
                var trimmed = x.Trim();
                return trimmed.StartsWith(".") ? trimmed : "." + trimmed;
            })
            .ToList();

        _allowedExtensions = new HashSet<string>(allowed, StringComparer.OrdinalIgnoreCase);

        var root = string.IsNullOrWhiteSpace(_options.RootPath) ? "./files" : _options.RootPath.Trim();

        _rootFullPath = Path.IsPathRooted(root)
            ? Path.GetFullPath(root)
            : Path.GetFullPath(Path.Combine(env.ContentRootPath, root));

        Directory.CreateDirectory(_rootFullPath);

        _logger.LogInformation("FileStorage RootPath resolved to: {Root}", _rootFullPath);
    }

    public async Task<FileItemResponse> SaveAsync(IFormFile file, string? scope, int? createdByAccountId, CancellationToken ct)
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

        var id = Guid.NewGuid();
        var fileId = id.ToString("N");
        var storedName = $"{fileId}{ext}";
        var storedPath = CombineUnderRoot(scope, storedName);

        // 1) Write bytes to disk
        try
        {
            await using (var stream = new FileStream(
                storedPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 64 * 1024,
                useAsync: true))
            {
                await file.CopyToAsync(stream, ct);
            }
        }
        catch
        {
            // If disk write fails, make sure we don't leave a partial file
            SafeDeleteFile(storedPath);
            throw;
        }

        // 2) Compute content type
        if (!_contentTypes.TryGetContentType(storedPath, out var contentType))
            contentType = file.ContentType ?? "application/octet-stream";

        // 3) Save metadata to DB
        // NOTE: CreatedByAccountId can be set later once you wire JWT -> AccountId.
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

        try
        {
            _db.FileAssets.Add(asset);
            await _db.SaveChangesAsync(ct);
        }
        catch
        {
            // DB write failed - delete the stored bytes so we don't orphan disk files
            SafeDeleteFile(storedPath);
            throw;
        }

        // 4) Return DTO (Url is filled by controller)
        return new FileItemResponse
        {
            Id = asset.Id.ToString("N"),
            OriginalName = asset.OriginalName,
            ContentType = asset.ContentType,
            Size = asset.Size,
            CreatedUtc = asset.CreatedUtc,
            Scope = asset.Scope,
            Url = null
        };
    }

    public async Task<(Stream Stream, FileItemResponse Meta)> OpenReadAsync(string fileId, CancellationToken ct)
    {
        var asset = await GetAssetRequiredAsync(fileId, ct);

        var fullPath = CombineUnderRoot(asset.Scope, asset.StoredName);
        if (!File.Exists(fullPath))
            throw new FileNotFoundException("Stored file not found.");

        var stream = new FileStream(
            fullPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 64 * 1024,
            useAsync: true);

        var meta = Map(asset);
        return (stream, meta);
    }

    public async Task<FileItemResponse?> GetMetaAsync(string fileId, CancellationToken ct)
    {
        if (!IsValidId(fileId)) return null;

        var id = Guid.ParseExact(fileId, "N");

        var asset = await _db.FileAssets
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return asset == null ? null : Map(asset);
    }

    public async Task<bool> DeleteAsync(string fileId, CancellationToken ct)
    {
        if (!IsValidId(fileId)) return false;

        var id = Guid.ParseExact(fileId, "N");

        var asset = await _db.FileAssets.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (asset == null) return false;

        var fullPath = CombineUnderRoot(asset.Scope, asset.StoredName);

        try
        {
            if (File.Exists(fullPath))
                File.Delete(fullPath);

            _db.FileAssets.Remove(asset);
            await _db.SaveChangesAsync(ct);

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

        var query = _db.FileAssets.AsNoTracking().AsQueryable();

        if (normalizedScope != null)
            query = query.Where(x => x.Scope != null && x.Scope == normalizedScope);

        var assets = await query
            .OrderByDescending(x => x.CreatedUtc)
            .ToListAsync(ct);

        return assets.Select(Map).ToList();
    }

    private async Task<FileAsset> GetAssetRequiredAsync(string fileId, CancellationToken ct)
    {
        if (!IsValidId(fileId))
            throw new FileNotFoundException("File not found.");

        var id = Guid.ParseExact(fileId, "N");

        var asset = await _db.FileAssets.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (asset == null)
            throw new FileNotFoundException("File not found.");

        return asset;
    }

    private static FileItemResponse Map(FileAsset asset) =>
        new FileItemResponse
        {
            Id = asset.Id.ToString("N"),
            OriginalName = asset.OriginalName,
            ContentType = asset.ContentType,
            Size = asset.Size,
            CreatedUtc = asset.CreatedUtc,
            Scope = asset.Scope,
            Url = null
        };

    private static bool IsValidId(string fileId) =>
        !string.IsNullOrWhiteSpace(fileId) && IdRegex.IsMatch(fileId);

    private string CombineUnderRoot(string? scope, string fileName)
    {
        var safeName = Path.GetFileName(fileName);
        var safeScope = string.IsNullOrWhiteSpace(scope)
            ? null
            : Regex.Replace(scope.Trim(), "[^a-zA-Z0-9_-]", "").ToLowerInvariant();

        var baseDir = _rootFullPath;
        if (!string.IsNullOrWhiteSpace(safeScope))
        {
            baseDir = Path.Combine(baseDir, safeScope);
            Directory.CreateDirectory(baseDir);
        }

        var combined = Path.GetFullPath(Path.Combine(baseDir, safeName));

        // IMPORTANT: Ensure it stays inside root
        if (!combined.StartsWith(_rootFullPath, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Invalid path resolution.");

        return combined;
    }

    private void SafeDeleteFile(string fullPath)
    {
        try
        {
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed cleanup delete: {Path}", fullPath);
        }
    }
}
