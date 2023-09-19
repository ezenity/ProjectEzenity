using Microsoft.AspNetCore.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ezenity_Backend.Helpers
{
    /// <summary>
    /// Configures JSON serialization options for the application.
    /// </summary>
    public class AppConfiguration
    {
        /// <summary>
        /// Configures JSON serialization options.
        /// </summary>
        /// <param name="options">The JSON options to configure.</param>
        public static void ConfigureJsonOptions(JsonOptions options)
        {
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.WriteIndented = true;
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        }
    }
}
