using Ezenity.DTOs.Models.Accounts;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ezenity.API.Filters
{
    /// <summary>
    /// Operation filter for modifying Swagger documentation related to the 'CreateSection' operation.
    /// </summary>
    public class UpdateAccountFilter : IOperationFilter
    {
        /// <summary>
        /// Applies modifications to the OpenAPI operation for the 'UpdateAccount' endpoint.
        /// </summary>
        /// <param name="operation">The OpenAPI operation object.</param>
        /// <param name="context">Context that provides schema and other information.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.OperationId != "UpdateAccount")
                return;

            operation.RequestBody.Content.Add("application/Ezenity.api.updatepartialaccount+json", new OpenApiMediaType()
            {
                Schema = context.SchemaGenerator.GenerateSchema(typeof(UpdateAccountRequest), context.SchemaRepository)
            });
        }
    }
}
