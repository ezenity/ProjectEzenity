using AutoMapper;
using Ezenity.Core.Entities.Accounts;
using Ezenity.Core.Interfaces;
using Ezenity.DTOs.Models.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezenity.Infrastructure.Helpers
{
    public class RoleResolver : IValueResolver<object, Account, Role>
    {
        private readonly IDataContext _context;

        public RoleResolver(IDataContext context)
        {
            _context = context;
        }

        public Role Resolve(object source, Account destination, Role destMember, ResolutionContext context)
        {
            // Extract the role name from the source object
            // Assuming all source types have a 'Role' property; Might need to adjust, needs more testing
            var roleName = (string)context.Items["Role"];
            var role = _context.Roles.FirstOrDefault(r => r.Name == roleName);
            if (role == null)
            {
                throw new InvalidOperationException($"Role with name '{roleName}' not found.");
            }
            return role;
        }
    }
}
