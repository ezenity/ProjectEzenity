using System;

namespace Ezenity.DTOs.Models.Files
{
    public class FileItemResponse
    {
        public string Id { get; set; }           // GUID (N format)
        public string OriginalName { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
        public DateTime CreatedUtc { get; set; }

        // Helpful for frontend to render directly
        public string Url { get; set; }
        public string? Scope { get; set; }       // "profile" | "vault" | "emblem" etc.
    }
}
