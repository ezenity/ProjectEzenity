using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace Ezenity.API.Filters
{
    /// <summary>
    /// Ensures x-api-version is sent as a HEADER, not as a Content-Type parameter.
    /// </summary>
    public class AddApiVersionHeaderOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= new List<OpenApiParameter>();

            // Prevent duplicates
            if (operation.Parameters.Any(p => p.Name == "x-api-version"))
                return;

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "x-api-version",
                In = ParameterLocation.Header,
                Required = true,
                Description = "API version (must be sent as a header, not in Content-Type).",
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Default = new OpenApiString("1.0")
                }
            });
        }
    }
}
