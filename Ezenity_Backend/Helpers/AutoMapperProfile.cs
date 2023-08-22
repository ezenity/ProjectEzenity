using AutoMapper;
using Ezenity_Backend.Entities.Accounts;
using Ezenity_Backend.Entities.EmailTemplates;
using Ezenity_Backend.Entities.Sections;
using Ezenity_Backend.Models.Accounts;
using Ezenity_Backend.Models.Emails;
using Ezenity_Backend.Models.Sections;

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

            // Email Templates
            CreateMap<EmailTemplate, EmailTemplateResponse>();
            CreateMap<CreateEmailTemplateRequest, EmailTemplate>();
            CreateMap<UpdateEmailTemplateRequest, EmailTemplate>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // Ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                        return true;
                    }
                ));

            // Sections
            CreateMap<Section, SectionResponse>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.ContentType))
                .ForMember(dest => dest.Layout, opt => opt.MapFrom(src => src.Layout));

            CreateMap<CreateSectionRequest, Section>();
            CreateMap<UpdateSectionRequest, Section>()
                .ForMember(dest => dest.Content, opt => opt.Ignore())
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
