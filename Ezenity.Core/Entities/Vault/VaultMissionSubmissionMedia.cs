using System;
using Ezenity.Core.Entities.Files;

namespace Ezenity.Core.Entities.Vault;

public class VaultMissionSubmissionMedia
{
    public int Id { get; set; }

    public int VaultSubmissionId { get; set; }
    public VaultMissionSubmission Submission { get; set; } = null!;

    // Link to stored file metadata (your FileAsset entity)
    public Guid FileAssetId { get; set; }
    public FileAsset FileAsset { get; set; } = null!;

    public string? Caption { get; set; }
    public int SortOrder { get; set; }

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
