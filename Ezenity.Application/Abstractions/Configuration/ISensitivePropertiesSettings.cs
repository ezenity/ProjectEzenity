namespace Ezenity.Application.Abstractions.Configuration;

/// <summary>
/// Defines the configuration for properties that should be considered sensitive.
/// These properties will be excluded from error metadata to prevent sensitive information leakage.
/// </summary>
public interface ISensitivePropertiesSettings
{
    /// <summary>
    /// Gets the list of property names that are considered sensitive.
    /// </summary>
    List<string> SensitiveErrorProperties { get; }
}
