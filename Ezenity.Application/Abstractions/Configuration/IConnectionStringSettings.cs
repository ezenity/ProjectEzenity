namespace Ezenity.Application.Abstractions.Configuration;

/// <summary>
/// Contains settings for the applications connection strings
/// </summary>
public interface IConnectionStringSettings
{
    /// <summary>
    /// Gets the SQL Connection details.
    /// </summary>
    string WebApiDatabase { get; }
}
