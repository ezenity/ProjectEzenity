using Ezenity_Backend.Entities.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ezenity_Backend.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        // Dynamic Roles
        private readonly IList<string> _roles;

        public AuthorizeAttribute(params string[] roleNames)
        {
            _roles = roleNames ?? new string[] { };
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            IAccount account = (IAccount)context.HttpContext.Items["Account"];


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
