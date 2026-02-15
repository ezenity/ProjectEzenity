using Ezenity.Domain.Entities.Accounts;

namespace Ezenity.Domain.Entities.Vault;

public class VaultMissionComment
{
    public Guid Id { get; set; }
    public int VaultMissionId { get; set; }
    public VaultMission VaultMission { get; set; } = null!;
    public int? AccountId { get; set; }
    public Account? Account { get; set; }
    public Guid? ParentCommentId { get; set; }
    public VaultMissionComment? ParentComment { get; set; }
    public ICollection<VaultMissionComment> Replies { get; set; } = new List<VaultMissionComment>();
    public string Body { get; set; } = null!;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
