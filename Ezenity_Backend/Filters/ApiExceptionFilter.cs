using Ezenity_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;

namespace Ezenity_Backend.Filters
{
    /// <summary>
    /// Provides global exception handling for API responses by capturing any thrown exceptions
    /// and converting them into a standardized JSON response format.
    /// </summary>
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        /// <summary>
        /// Called when an exception occurs in the application. Overrides the base method to provide
        /// a custom response in case of an exception.
        /// </summary>
        /// <param name="context">
        /// The <see cref="ExceptionContext"/> encapsulating the exception details and other 
        /// contextual information.
        /// </param>
        public override void OnException(ExceptionContext context)
        {
            // Create an instance of ApiResponse with a default error message and status code.
            ApiResponse<string> apiResponse = new ApiResponse<string>
            {
                IsSuccess = false,
                Message = "An unexpected error occurred.",
                StatusCode = 500,
                // Include the exception message in the errors list.
                Errors = new List<string> { context.Exception.Message }
            };

            // Set the JSON result with the ApiResponse object.
            context.Result = new JsonResult(apiResponse)
            {
                StatusCode = apiResponse.StatusCode
            };
        }
    }
}
