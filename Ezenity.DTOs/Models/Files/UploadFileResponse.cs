namespace Ezenity.DTOs.Models.Files
{
    /// <summary>
    /// Response wrapper for upload endpoint.
    /// </summary>
    public class UploadFileResponse
    {
        public FileItemResponse File { get; set; } = default!;
    }
}
