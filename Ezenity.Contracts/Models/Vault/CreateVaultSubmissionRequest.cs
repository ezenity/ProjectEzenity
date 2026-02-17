namespace Ezenity.Contracts.Models.Vault;

public class CreateVaultSubmissionRequest
{
    public string? ProofYoutubeUrl { get; set; }
    public string? ProofInstagramUrl { get; set; }
    public string? ProofTiktokUrl { get; set; }
    public string? ProofFacebookUrl { get; set; }
    public string? Notes { get; set; }

    public List<string> FileIds { get; set; } = new();
}
