namespace Ezenity.Domain.Entities.Accounts;

/// <summary>
/// Represents a role in the system.
/// </summary>
public class Role
{
    /// <summary>
    /// Gets or sets the role ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the role.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the asociated accounts with the specified Role
    /// </summary>
    public List<Account> Accounts { get; set; } = new List<Account>();
}
