using System;
using System.Collections.Generic;

namespace Ezenity.API.Options
{
    /// <summary>
    /// Configuration settings for file storage (local filesystem).
    /// Bind from configuration section: "FileStorage".
    ///
    /// Environment variable equivalents:
    /// - FileStorage__RootPath
    /// - FileStorage__MaxUploadBytes
    /// - FileStorage__AllowedExtensions__0, __1, ...
    /// </summary>
    public sealed class FileStorageOptions
    {
        /// <summary>
        /// Absolute or relative path where files are stored.
        /// If relative, it is resolved against the application content root.
        /// </summary>
        public string RootPath { get; set; } = "./files";

        /// <summary>
        /// Maximum allowed upload size in bytes (per request).
        /// Controller-level limits can also be applied via [RequestSizeLimit].
        /// Default: 50 MB.
        /// </summary>
        public long MaxUploadBytes { get; set; } = 50_000_000;

        /// <summary>
        /// List of allowed file extensions, including the leading dot.
        /// Example: ".jpg", ".mp4"
        /// </summary>
        public List<string> AllowedExtensions { get; set; } = new()
        {
            ".jpg", ".jpeg", ".png", ".webp", ".gif",
            ".mp4", ".webm", ".mov"
        };
    }
}
