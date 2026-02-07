using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Ezenity.API.Controllers
{
    /// <summary>
    /// Handles API operations related to roles within the system.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/roles")]
    [ApiVersion("2.0")]
    [Produces("application/vnd.api+json", "application/json", "application/xml")]
    public class RolesController
    {
    }
}
