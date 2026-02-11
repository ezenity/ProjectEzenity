using System;
using System.Collections.Generic;

namespace Ezenity.Core.Entities.Vault;

public class VaultMission
{
    public int Id { get; set; }

    public string Slug { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Summary { get; set; } = null!;
    public string Description { get; set; } = null!; // markdown or plain text

    /// <summary>
    /// Store objectives as JSON for now (keeps it flexible while you iterate).
    /// </summary>
    public string? ObjectivesJson { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    public ICollection<VaultMissionReward> Rewards { get; set; } = new List<VaultMissionReward>();
    public ICollection<VaultMissionSubmission> Submissions { get; set; } = new List<VaultMissionSubmission>();
    public ICollection<VaultMissionCompletion> Completions { get; set; } = new List<VaultMissionCompletion>();
    public ICollection<VaultMissionComment> Comments { get; set; } = new List<VaultMissionComment>();

}
