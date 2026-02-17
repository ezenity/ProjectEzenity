using Ezenity.Application.Abstractions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ezenity.API.Middleware
{
    /// <summary>
    /// Middleware for handling exceptions globally and converting them into structured, 
    /// JSON:API compliant HTTP responses. It logs errors and provides a consistent error handling strategy.
    /// </summary>
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IAppSettings _appSettings;
        private readonly ISensitivePropertiesSettings _sensitivePropertiesSettings;

        /// <summary>
        /// Initializes a new instance of the ErrorHandlerMiddleware class with dependencies.
        /// </summary>
        /// <param name="next">The next delegate in the middleware pipeline.</param>
        /// <param name="logger">Logger for diagnostic messages.</param>
        /// <param name="appSettings">Application settings for retrieving configuration like base URL.</param>
        /// <param name="sensitivePropertiesSettings">Settings to identify sensitive properties that should be excluded from error logs.</param>
        public ErrorHandlerMiddleware(
            RequestDelegate next,
            ILogger<ErrorHandlerMiddleware> logger,
            // IOptions<IAppSettings> appSettings, // Causing a 'MissingMethodException'
            // IOptionsSnapshot<IAppSettings> appSettings, // Can update configuration data when the application reads the data.
            // IOptionsMonitor<IAppSettings> appSettings, // Provides a change notification when the 'IAppSettings' data changes.
            IAppSettings appSettings, // Directly injecting IAppSetings
            ISensitivePropertiesSettings sensitivePropertiesSettings)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            // _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings)); Used for IOptions...
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            _sensitivePropertiesSettings = sensitivePropertiesSettings ?? throw new ArgumentNullException(nameof(sensitivePropertiesSettings));
        }

        /// <summary>
        /// Processes a request and catches any unhandled exceptions to format them into structured JSON:API error responses.
        /// </summary>
        /// <param name="context">The HTTP context for the current request.</param>
        /// <returns>A task representing the asynchronous operation of exception handling and response generation.</returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                await HandleExceptionAsync(context, error);
            }
        }

        /// <summary>
        /// Handles exceptions by determining the appropriate HTTP status code and creating a JSON:API error response.
        /// </summary>
        /// <param name="context">The HTTP context for the current request.</param>
        /// <param name="exception">The caught exception.</param>
        /// <returns>A task representing the asynchronous operation of error response writing.</returns>
        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Default to 500 if unexpected
            var code = HttpStatusCode.InternalServerError;
            var errors = new List<JsonApiErrors>();
            var response = context.Response;
            response.ContentType = "application/vnd.api+json";

            switch (exception)
            {
                case NotFoundException notFoundException:
                    // 404 NotFound: Resource requested doesn't exist, typically for specific items or entities.
                    code = HttpStatusCode.NotFound;
                    errors.Add(CreateJsonApiError(notFoundException, code, context));
                    break;

                case AuthenticationException authenticationException:
                    // 401 Unauthorized: Request lacks valid authentication credentials. It may need to authenticate or re-authenticate.
                    code = HttpStatusCode.Unauthorized;
                    errors.Add(CreateJsonApiError(authenticationException, code, context));
                    break;

                case AuthorizationException authorizationException:
                    // 403 Forbidden: The client is authenticated but not authorized to perform the requested action.
                    code = HttpStatusCode.Forbidden;
                    errors.Add(CreateJsonApiError(authorizationException, code, context));
                    break;

                // TODO: Split into more explicit exceptions with different status codes
                //      400 Bad Request - If the business rule violation is due to the client sending a request that doesn't conform to your business logic.
                //      412 Precondition Failed - If the violation is due to a precondition specified in the request headers not being met.
                case BusinessRuleException businessRuleException:
                    // 409 Conflict: Request could not be processed because of a conflict in the request, such as an edit conflict.
                    code = HttpStatusCode.Conflict;
                    errors.Add(CreateJsonApiError(businessRuleException, code, context));
                    break;

                // TODO: Split into more explicit exceptions with different status codes
                //      412 Precondition Failed - If the error occurs because the client's version of a resource is out of date.
                case ConcurrencyException concurrencyException:
                    // 409 Conflict: Multiple requests conflicted with each other, typically in scenarios involving versioning and simultaneous updates.
                    code = HttpStatusCode.Conflict;
                    errors.Add(CreateJsonApiError(concurrencyException, code, context));
                    break;

                // TODO: Split into more explicit exceptions with different status codes
                //      500 Internal Server Error - If the issue is due to an unexpected problem within your data layer rather than the service being unavailable.
                //      400 Bad Request - If the data access issue is due to the client's request being malformed or containing invalid data.
                case DataAccessException dataAccessException:
                    // 503 Service Unavailable: The server is unable to access required data. This can be due to a database outage or other data access issues.
                    code = HttpStatusCode.ServiceUnavailable;
                    errors.Add(CreateJsonApiError(dataAccessException, code, context));
                    break;

                // TODO: Split into more explicit exceptions with different status codes
                //      409 Conflict - If the deletion failed due to a conflict with the current state of the resource (e.g., it's being used by another process or has related data that prevents deletion).
                case DeletionFailedException deletionFailedException:
                    // 400 Bad Request: The request to delete something was invalid or could not be processed, perhaps due to state or dependency issues.
                    code = HttpStatusCode.BadRequest;
                    errors.Add(CreateJsonApiError(deletionFailedException, code, context));
                    break;

                // TODO: Split into more explicit exceptions with different status codes
                //      502 Bad Gateway - If the issue is specifically with an upstream service or dependency rather than your service being generally unavailable.
                //      504 Gateway Timeout - If the dependency is not responding in a timely manner.
                case DependencyFailureException dependencyFailureException:
                    // 503 Service Unavailable: The server is unable to reach or process a request due to a failure in a service or network it depends on.
                    code = HttpStatusCode.ServiceUnavailable;
                    errors.Add(CreateJsonApiError(dependencyFailureException, code, context));
                    break;

                case InvalidVerificationTokenException invalidVerificationTokenException:
                    // 400 Bad Request: The request contains an invalid token, such as a verification token that is malformed or expired.
                    code = HttpStatusCode.BadRequest;
                    errors.Add(CreateJsonApiError(invalidVerificationTokenException, code, context));
                    break;

                case RateLimitExceededException rateLimitExceededException:
                    // 429 Too Many Requests: The client has sent too many requests in a given amount of time ("rate limiting").
                    code = HttpStatusCode.TooManyRequests;
                    errors.Add(CreateJsonApiError(rateLimitExceededException, code, context));
                    break;

                case ResourceAlreadyExistsException resourceAlreadyExistsException:
                    // 409 Conflict: Attempt to create a resource that already exists. Useful for scenarios where duplication is not allowed.
                    code = HttpStatusCode.Conflict;
                    errors.Add(CreateJsonApiError(resourceAlreadyExistsException, code, context));
                    break;

                case ResourceNotFoundException resourceNotFoundException:
                    // 404 NotFound: A more specific resource within the operation was not found, similar to NotFoundException but for specific scenarios.
                    code = HttpStatusCode.NotFound;
                    errors.Add(CreateJsonApiError(resourceNotFoundException, code, context));
                    break;

                // TODO: Split into more explicit exceptions with different status codes
                //      400 Bad Request - While 422 is more specific to validation issues, 400 is a more general-purpose status for any kind of bad request, which might be suitable for this API's design.
                case ValidationException validationException:
                    // 422 Unprocessable Entity: The server understands the content type of the request entity, and the syntax is correct, but it was unable to process the contained instructions.
                    code = HttpStatusCode.UnprocessableEntity;
                    errors.Add(CreateJsonApiError(validationException, code, context));
                    break;

                case AppException appException:
                    // 400 Bad Request: General client-side error for application-specific issues not covered by other exceptions.
                    code = HttpStatusCode.BadRequest;
                    errors.Add(CreateJsonApiError(appException, code, context));
                    break;

                default:
                    // 500 Internal Server Error: A catch-all for unexpected server-side errors. Logs the error and returns a generic error response.
                    _logger.LogError(exception, "An unhandled exception has occurred: {ExceptionMessage}", exception.Message);
                    errors.Add(CreateJsonApiError(exception, code, context));
                    break;
            }

            var verboseApiResponse = new VerboseApiResponse<object>
            {
                StatusCode = (int) code,
                Message = exception.Message,
                IsSuccess = false,
                Errors = errors,
                DebugInfo = exception.StackTrace,
                TimeGenerated = DateTime.UtcNow
            };

            response.StatusCode = (int) code;

            return context.Response.WriteAsync(JsonSerializer.Serialize(verboseApiResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
        }

        /// <summary>
        /// Creates a JSON:API compliant error object based on the provided exception and HTTP status code.
        /// It excludes any sensitive properties from the error details.
        /// </summary>
        /// <param name="exception">The exception to convert into a JSON:API error.</param>
        /// <param name="statusCode">The HTTP status code associated with the error.</param>
        /// <param name="context">The HTTP context for the current request.</param>
        /// <returns>A JSON:API error object representing the provided exception.</returns>
        private JsonApiErrors CreateJsonApiError(Exception exception, HttpStatusCode statusCode, HttpContext context)
        {
            var baseUrl = _appSettings.BaseUrl ?? $"{context.Request.Scheme}://{context.Request.Host}";
            var error = new JsonApiErrors
            {
                Id = Guid.NewGuid().ToString(),
                Status = ((int)statusCode).ToString(),
                Title = "Error: " + exception.GetType().Name.Replace("Exception", ""),
                Detail = exception.Message,
                Links = new JsonApiErrors.ErrorLinks
                {
                    About = $"{baseUrl}/errors/{exception.GetType().Name}"
                },
                Meta = new Dictionary<string, object>
                {
                    { "timestamp", DateTime.UtcNow.ToString("o") },
                }
            };

            var sensitiveProperties = new HashSet<string>(_sensitivePropertiesSettings.SensitiveErrorProperties);

            // Use reflection to add all public properties of the exception to the Meta dictionary.
            foreach (PropertyInfo propInfo in exception.GetType().GetProperties())
            {
                if (propInfo.CanRead)
                {
                    string name = propInfo.Name;

                    // Skip sensitive properties
                    if (sensitiveProperties.Contains(name))
                    {
                        continue;
                    }

                    Object value = propInfo.GetValue(exception, null);

                    // Ensure that the value can be serialized and is not a complex object
                    if (value is null || value is string || value.GetType().IsValueType)
                    {
                        error.Meta.Add(name, value);
                    }
                }
            }

            return error;
        }

    }
}
