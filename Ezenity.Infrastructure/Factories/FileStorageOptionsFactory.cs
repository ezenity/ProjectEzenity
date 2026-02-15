using Ezenity.Domain.Options;
using Microsoft.Extensions.Configuration;

namespace Ezenity.Infrastructure.Factories;

public static class FileStorageOptionsFactory
{
    public static FileStorageOptions Create(IConfiguration configuration)
    {
        // 1) Try standard binder path first: "FileStorage"
        var bound = new FileStorageOptions();
        configuration.GetSection("FileStorage").Bind(bound);

        // If RootPath got bound (or anything else), we still allow env overrides below.

        // 2) Optional overrides via your custom env vars
        // Root
        var root = Environment.GetEnvironmentVariable("EZENITY_FILES_ROOT");
        if (!string.IsNullOrWhiteSpace(root))
            bound.RootPath = root;

        // Max bytes
        var maxBytes = Environment.GetEnvironmentVariable("EZENITY_FILES_MAX_UPLOAD_BYTES")
                      ?? Environment.GetEnvironmentVariable("EZENITY_FILES_MAX_BYTES");
        if (!string.IsNullOrWhiteSpace(maxBytes) && long.TryParse(maxBytes, out var parsed))
            bound.MaxUploadBytes = parsed;

        // Allowed extensions (comma-separated)
        // Example: ".jpg,.png,.mp4"
        var allowed = Environment.GetEnvironmentVariable("EZENITY_FILES_ALLOWED_EXTENSIONS");
        if (!string.IsNullOrWhiteSpace(allowed))
        {
            var list = allowed
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(x => x.StartsWith('.') ? x.ToLowerInvariant() : "." + x.ToLowerInvariant())
                .Distinct()
                .ToList();

            if (list.Count > 0)
                bound.AllowedExtensions = list;
        }

        // Normalize RootPath
        bound.RootPath = bound.RootPath.Trim();

        return bound;
    }
}
