namespace Ezenity.Domain.Entities.Vault;

public class RideVerificationResult
{
    public Guid Id { get; set; }

    public Guid MissionsessionId { get; set; }
    public MissionSession MissionSession { get; set; } = null!;

    public int GpsIntegrityScore { get; set; }
    public int TravelPatternScore { get; set; }
    public int MotionSignatureScore { get; set; }
    public int HistoricalProfileScore { get; set; }
    public int AudioConfidenceScore { get; set; }

    public int TotalRideConfidenceScore { get; set; }

    public bool HardFailTriggered { get; set; }
    public string? DecisionReason { get; set; }

    public VerificationDecision Decision { get; set; }
    public string? DecisionReason { get; set; }

    public bool RequiresManualReview { get; set; }

    public DateTime EvaluatedUtc { get; set; } = DateTime.UtcNow;
}
