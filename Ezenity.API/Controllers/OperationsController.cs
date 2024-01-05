using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace Ezenity.API.Controllers
{
    /// <summary>
    /// Provides an API controller for system operations like reloading configurations.
    /// </summary>
    [ApiController]
    [Route("api/operations")]
    [ApiVersion("2.0")]
    [Produces("application/json", "application/xml")]
    public class OperationsController : ControllerBase
    {
        /// <summary>
        /// Represents the application's current configuration as a set of key-value pairs.
        /// </summary>
        private readonly IConfiguration _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationsController"/> class.
        /// </summary>
        /// <param name="config">The application's current configuration.</param>
        public OperationsController(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Reloads the application's configuration settings from all registered providers.
        /// </summary>
        /// <returns>
        /// A status code representing the result of the operation.
        /// - Ok (HTTP 200) if the operation succeeds.
        /// - Internal Server Error (HTTP 500) if an exception occurs.
        /// </returns>
        /// <remarks>
        /// Make an HTTP OPTIONS request to this endpoint to reload the application's configuration.
        /// </remarks>
        /// <exception cref="Exception">Thrown when an error occurs during configuration reload.</exception>
        [HttpOptions("reloadconfig")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public IActionResult ReloadConfig()
        {
            try
            {
                var root = (IConfigurationRoot) _config;
                root.Reload();
                return Ok();
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
