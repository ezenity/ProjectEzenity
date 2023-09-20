using Ezenity_Backend.Entities.Accounts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ezenity_Backend.Attributes
{
    /// <summary>
    /// Custom attribute to handle authorization for specified roles. 
    /// Implements the <see cref="IAuthorizationFilter"/> interface to handle custom authorization logic.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        /// <summary>
        /// Internal field holding a list of roles that are allowed to access the resource.
        /// </summary>
        private readonly IList<string> _roles;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeAttribute"/> class.
        /// </summary>
        /// <param name="roleNames">An array of role names that are authorized to access the resource.</param>
        public AuthorizeAttribute(params string[] roleNames)
        {
            _roles = roleNames ?? new string[] { };
        }

        /// <summary>
        /// Handles the authorization logic. This method is called by the framework before the controller action is invoked.
        /// </summary>
        /// <param name="context">Provides access to the <see cref="HttpContext"/>, action descriptors, and more.</param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            Account account = (Account)context.HttpContext.Items["Account"];


            if (account == null)
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                return;
            }

            if (_roles.Any())
            {
                var accountRoleName = account.Role?.Name;  // Assumes Role is not null and has a Name property.
                if (string.IsNullOrEmpty(accountRoleName) || !_roles.Contains(accountRoleName))
                {
                    context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                    return;
                }
            }
        }
    }
}
