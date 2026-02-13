using System;
using Ezenity.Core.Entities.Accounts;

namespace Ezenity.Core.Entities.Files;

/// <summary>
/// File metadata stored in the database.
/// Bytes live on disk (LocalFileStorageService).
/// </summary>
public class FileAsset
{
    public Guid Id { get; set; }

    public string OriginalName { get; set; } = default!;
    public string StoredName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public long Size { get; set; }

    public string? Scope { get; set; } // "vault", "profile", "emblems", etc.
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    public int? CreatedByAccountId { get; set; }
    public Account? CreatedByAccount { get; set; }
}
