namespace Ezenity.Domain.Entities.Vault;

public class VaultMissionReward
{
    public int Id { get; set; }
    public int VaultMissionId { get; set; }
    public VaultMission Mission { get; set; } = null!;
    public int Coins { get; set; } = 0;
    public int Rep { get; set; } = 0;
}
