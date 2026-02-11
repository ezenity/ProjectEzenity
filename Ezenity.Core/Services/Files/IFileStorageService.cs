using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Ezenity.API.Models.Files;

namespace Ezenity.Core.Services.Files
{
    /// <summary>
    /// Handles storing and retrieving files (and metadata) independent of controllers.
    /// </summary>
    public interface IFileStorageService
    {
        Task<FileItemResponse> SaveAsync(IFormFile file, string? scope, CancellationToken ct);
        Task<(Stream Stream, FileItemResponse Meta)> OpenReadAsync(string fileId, CancellationToken ct);
        Task<FileItemResponse?> GetMetaAsync(string fileId, CancellationToken ct);
        Task<bool> DeleteAsync(string fileId, CancellationToken ct);
        Task<IReadOnlyList<FileItemResponse>> ListAsync(string? scope, CancellationToken ct);
    }
}
