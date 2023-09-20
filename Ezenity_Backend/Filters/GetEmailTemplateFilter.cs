using Ezenity_Backend.Models.EmailTemplates;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace Ezenity_Backend.Filters
{
    /// <summary>
    /// Operation filter for modifying Swagger documentation related to the 'GetEmailTemplate' operation.
    /// </summary>
    public class GetEmailTemplateFilter : IOperationFilter
    {
        /// <summary>
        /// Applies modifications to the OpenAPI operation for the 'GetEmailTemplate' endpoint.
        /// </summary>
        /// <param name="operation">The OpenAPI operation object.</param>
        /// <param name="context">Context that provides schema and other information.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.OperationId != "GetEmailTemplate")
                return;

            var schema = context.SchemaGenerator.GenerateSchema(typeof(EmailTemplateResponse), context.SchemaRepository);

            if (operation.Responses.Any(response => response.Key == StatusCodes.Status200OK.ToString()))
                operation.Responses[StatusCodes.Status200OK.ToString()].Content.Add("application/ezenity.api.getemailtemplatenondynamiccontent+json", new OpenApiMediaType() { Schema = schema });
        }
    }
}
