namespace Ezenity.Domain.Entities.Vault;

public class MissionReward
{
    public Guid Id { get; set; }
    
    public Guid VaultMissionId { get; set; }
    public Mission Mission { get; set; } = null!;

    public MissionRewardType RewardType { get; set; }

    public int Amount { get; set; }
    public string? Value { get; set; } // emblem code, vault key, etc..
    public bool IsGuaranteed { get; set; } = true;

    public int Coins { get; set; } = 0;
    public int Rep { get; set; } = 0;
}
