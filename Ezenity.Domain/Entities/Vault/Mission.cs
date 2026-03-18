namespace Ezenity.Domain.Entities.Vault;

public class Mission
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!; // ORL-INTRO-001  
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!; // markdown or plain text

    public Guid ChapterId { get; set; }
    public Chapter Chapter { get; set; } = null!;

    public MissionType Type { get; set; } // Intro, Route, Exploration, Certification, Streat
    public MissionDifficulty Difficulty { get; set; } // Basic, Intermediate, Advance, Elite

    public bool IsActive { get; set; } = true;
    public bool IsHidden { get; set; } = false;
    public bool RequiresManualReview { get; set; } = false;

    public int SortOrder { get; set; }

    public decimal StartLatitude { get; set; }
    public decimal StartLongitude { get; set; }
    public int StartRadiusMeters { get; set; }

    public decimal EndLatitude { get; set; }
    public decimal EndLongitude { get; set; }
    public int EndRadiusMeters { get; set; }

    public decimal? ExpectedDistanceMiles { get; set; }
    public int? MaxRouteDeviationMeters { get; set; }
    public int? MinDurationSeconds { get; set; }
    public int? MaxDurationSeconds { get; set; }

    public bool RequiresNightRide { get; set; }
    public bool RequiresMotionVerification { get; set; } = true;
    public bool AllowsAudioVerification { get; set; } = false;

    public int MinTierLevelRequired { get; set; }
    public int MinRepRequired { get; set; }

    public ICollection<MissionRequirement> Requirements { get; set; } = new List<MissionRequirement>();
    public ICollection<MissionReward> Rewards { get; set; } = new List<MissionReward>();


    
    
    /// ASK AI: Why the below was not added in the suggestion they gave aboe for `Mission.cs` code




    public string Slug { get; set; } = null!;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    // one-to-one numeric rewards
    public MissionReward Reward { get; set; } = new MissionReward();

    // emblem rewards (many-to-many via join)
    public ICollection<MissionEmblemReward> EmblemRewards { get; set; } = new List<MissionEmblemReward>();

    // submissions/comments/completions
    public ICollection<MissionSubmission> Submissions { get; set; } = new List<MissionSubmission>();
    public ICollection<MissionComment> Comments { get; set; } = new List<MissionComment>();
    public ICollection<MissionCompletion> Completions { get; set; } = new List<MissionCompletion>();
}
