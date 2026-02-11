namespace Ezenity.Core.Entities.Vault;

public enum VaultMediaType { Image = 0, Video = 1 }

public class VaultSubmissionMedia
{
    public int Id { get; set; }
    public int SubmissionId { get; set; }
    public VaultSubmission Submission { get; set; } = null!;

    public string FileId { get; set; } = null!; // references FileAsset.Id (string)
    public VaultMediaType MediaType { get; set; }
    public string? Caption { get; set; }
}
