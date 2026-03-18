namespace Ezenity.Domain.Entities.Vault;

public class MissionSessionPoint
{
    public Guid Id { get; set; }

    public Guid MissionSessionId { get; set; }
    public MissionSession MessionSession { get; set; } = null!;

    public DateTime RecordedUtc { get; set; }

    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    
    public decimal SpeedMph { get; set; }
    public decimal HeadingDegrees { get; set; }
    public decimal AccuracyMeters { get; set; }

    public decimal? AccelerometerMagnitude { get; set; }
    public decimal? GyroscopeMagnitude { get; set; }

    public bool IsInsideRouteCorridor { get; set; }
}
