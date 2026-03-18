using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezenity.Domain.Entities.Vault;

public class MissionSession
{
    public Guid Id { get; set; }

    public Guid MissionId { get; set; }
    public Mission Mission { get; set; } = null!;

    public Guid UserId { get; set; }

    public MissionSessionStatus Status { get; set; } = MissionSessionStatus.Activated;

    public DateTime ActivatedUtc { get; set; }
    public DateTime? StartedUtc { get; set; }
    public DateTime? EndedUtc { get; set; }

    public bool StartZoneEntered { get; set; }
    public bool EndZoneEntered { get; set; }

    public decimal? StartLatitude { get; set; }
    public decimal? StartLongitude { get; set; }

    public decimal? EndLatitude { get; set; }
    public decimal? EndLongitude { get; set; }

    public decimal DistanceMiles { get; set; }
    public int DurationSeconds { get; set; }

    public int StopCount { get; set; }
    public int RouteDeviationCount { get; set; }
    public decimal RouteAccuracyPercent { get; set; }

    public decimal AverageSpeedMph { get; set; }
    public decimal MaxSpeedMph { get; set; }

    public bool TeleportDetected { get; set; }
    public bool MockLocationSuspected { get; set; }

    public int TotalGpsPoints { get; set; }

    public bool UsedAudioVerification { get; set; }
    public bool UsedMotionVerification { get; set; }

    public string? FailureReason { get; set; }

    public Guid? RideVerificationResultId { get; set; }
    public RideVerificationResult? RideVerificationResult { get; set; }

    public MissionSessionCompletionType CompletionType { get; set; } = MissionSessionCompletionType.None;

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

}
