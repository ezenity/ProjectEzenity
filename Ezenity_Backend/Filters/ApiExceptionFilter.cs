using Ezenity_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;

namespace Ezenity_Backend.Filters
{
    //
    // Summary:
    //      Filter for global exception handling.
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            ApiResponse<string> apiResponse = new ApiResponse<string>
            {
                IsSuccess = false,
                Message = "An unexpected error occurred.",
                StatusCode = 500,
                Errors = new List<string> { context.Exception.Message }
            };

            context.Result = new JsonResult(apiResponse)
            {
                StatusCode = apiResponse.StatusCode
            };
        }
    }
}
