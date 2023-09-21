using Ezenity_Backend.Entities.Accounts;
using Ezenity_Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ezenity_Backend.Controllers
{
    /// <summary>
    /// Handles API operations related to roles within the system.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/roles")]
    [ApiVersion("2.0")]
    [Produces("application/json", "application/xml")]
    public class RolesController
    {
    }
}
