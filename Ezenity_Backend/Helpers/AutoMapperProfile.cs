using AutoMapper;
using Ezenity_Backend.Entities;
using Ezenity_Backend.Models.Accounts;
using Ezenity_Backend.Models.Skills;

namespace Ezenity_Backend.Helpers
{
    public class AutoMapperProfile : Profile
    {
        // Mappings between model and entity objects
        public AutoMapperProfile()
        {
            // Accounts
            CreateMap<Account, AccountResponse>();
            CreateMap<Account, AuthenticateResponse>();
            CreateMap<RegisterRequest, Account>();
            CreateMap<CreateRequest, Account>();
            CreateMap<UpdateRequest, Account>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // Ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                        // Ignore null role
                        if (x.DestinationMember.Name == "Role" && src.Role == null) return false;

                        return true;
                    }
                ));

            // Skills
            CreateMap<Skill, SkillResponse>();
            CreateMap<CreateSkillRequest, Skill>();
            CreateMap<UpdateSkillRequest, Skill>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // Ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                        return true;
                    }
                ));
        }
    }
}
