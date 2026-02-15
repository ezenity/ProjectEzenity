using Ezenity.Domain.Entities.Accounts;
using Ezenity.Domain.Entities.Files;

namespace Ezenity.Domain.Entities.Vault;

public class VaultMissionSubmission
{
    public int Id { get; set; }
    public string? Platform { get; set; }
    // Optional notes (creator notes or admin notes depending on how you use it)
    public string? Notes { get; set; }
    public VaultMission Mission { get; set; } = null!;
    public int VaultMissionId { get; set; }
    // The submitting user (Account PK)
    public int? SubmittedByAccountId { get; set; } // The Account PK
    public Account? SubmittedByAccount { get; set; }
    // Where the proof lives (youtube / instagram / tiktok / facebook / etc)
    // Link to the proof (optional if you allow direct site upload only)
    public string? ExternalUrl { get; set; }
    public DateTime SubmittedUtc { get; set; } = DateTime.UtcNow;
    // Single source of truth for status
    public VaultMissionSubmissionStatus Status { get; set; } = VaultMissionSubmissionStatus.Pending;
    // Admin review info
    public int? ReviewedByAccountId { get; set; }
    public Account? ReviewedByAccount { get; set; }
    public DateTime? ReviewedUtc { get; set; }
    public string? ReviewNote { get; set; }
    // Attachments for proof uploaded directly to site
    // Uploaded proof media (files)
    public ICollection<VaultMissionSubmissionMedia> Media { get; set; } = new List<VaultMissionSubmissionMedia>();
}
