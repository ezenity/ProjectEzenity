using Ezenity.Contracts.Models.Files;
using Microsoft.AspNetCore.Http;

namespace Ezenity.Application.Abstractions.Files;

/// <summary>
/// Handles storing and retrieving files (and metadata) independent of controllers.
/// 
/// Abstraction for storing and retrieving binary file content.
/// The database stores metadata; this service stores the actual bytes on disk.
/// </summary>
public interface IFileStorageService
{
    Task<FileItemResponse> SaveAsync(IFormFile file, string? scope, int? createdByAccountId, CancellationToken ct);
    Task<(Stream Stream, FileItemResponse Meta)> OpenReadAsync(string fileId, CancellationToken ct);
    Task<FileItemResponse?> GetMetaAsync(string fileId, CancellationToken ct);
    Task<bool> DeleteAsync(string fileId, CancellationToken ct);
    Task<IReadOnlyList<FileItemResponse>> ListAsync(string? scope, CancellationToken ct);
}
