using Ezenity.Domain.Entities.Accounts;

namespace Ezenity.Domain.Entities.Vault;

public class MissionComment
{
    public Guid Id { get; set; }
    public int VaultMissionId { get; set; }
    public Mission VaultMission { get; set; } = null!;
    public int? AccountId { get; set; }
    public Account? Account { get; set; }
    public Guid? ParentCommentId { get; set; }
    public MissionComment? ParentComment { get; set; }
    public ICollection<MissionComment> Replies { get; set; } = new List<MissionComment>();
    public string Body { get; set; } = null!;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
