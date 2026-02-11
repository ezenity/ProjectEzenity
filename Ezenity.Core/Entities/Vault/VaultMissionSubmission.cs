using System;
using Ezenity.Core.Entities.Accounts;
using Ezenity.Core.Entities.Files;

namespace Ezenity.Core.Entities.Vault;

public class VaultSubmission
{
    public int Id { get; set; }

    public int VaultMissionId { get; set; }
    public VaultMission Mission { get; set; } = null!;

    public int? SubmittedByAccountId { get; set; } // The Account PK
    public Account Account? SubmittedByAccount {  get; set; }

    public string? Platform { get; set; } // youtube, instagram, tiktok, etc.
    public string? ExternalUrl { get; set; }

    public DateTime SubmittedUtc { get; set; } = DateTime.UtcNow;

    public VaultSubmissionStatus Status { get; set; } = VaultSubmissionStatus.Pending;

    // Proof links
    //public string? ProofYoutubeUrl { get; set; }
    //public string? ProofInstagramUrl { get; set; }
    //public string? ProofTiktokUrl { get; set; }
    //public string? ProofFacebookUrl { get; set; }

    public string Status { get; set; } = "Pending"; // Pending/Approved/Rejected
    public string? Notes { get; set; }

    // Admin review
    public int? ReviewedByAccountId { get; set; }
    public DateTime? ReviewedUtc { get; set; }
    public string? ReviewNote { get; set; }

    public List<VaultSubmissionMedia> Media { get; set; } = new();
}
