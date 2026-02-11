using System;
using System.Collections.Generic;

namespace Ezenity.Core.Entities.Vault;

public enum VaultSubmissionStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2
}

public class VaultSubmission
{
    public int Id { get; set; }

    public int MissionId { get; set; }
    public VaultMission Mission { get; set; } = null!;

    public int AccountId { get; set; } // The Account PK
    public DateTime SubmittedUtc { get; set; } = DateTime.UtcNow;

    public VaultSubmissionStatus Status { get; set; } = VaultSubmissionStatus.Pending;

    // Proof links
    public string? ProofYoutubeUrl { get; set; }
    public string? ProofInstagramUrl { get; set; }
    public string? ProofTiktokUrl { get; set; }
    public string? ProofFacebookUrl { get; set; }

    public string? Notes { get; set; }

    // Admin review
    public int? ReviewedByAccountId { get; set; }
    public DateTime? ReviewedUtc { get; set; }
    public string? ReviewNote { get; set; }

    public List<VaultSubmissionMedia> Media { get; set; } = new();
}
