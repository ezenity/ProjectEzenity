using AutoMapper;
using Ezenity.Core.Entities.Accounts;
using Ezenity.Core.Entities.EmailTemplates;
using Ezenity.Core.Entities.Sections;
using Ezenity.DTOs.Models.Accounts;
using Ezenity.DTOs.Models.EmailTemplates;
using Ezenity.DTOs.Models.Sections;
using System;

namespace Ezenity.Infrastructure.Helpers
{
    /// <summary>
    /// Defines the AutoMapper profiles for mapping between entity and model objects.
    /// </summary>
    public class AutoMapperProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMapperProfile"/> class and configures object mappings.
        /// Mappings between model and entity objects
        /// </summary>
        public AutoMapperProfile()
        {
            // Accounts
            //  - Configures the mappings for Account-related objects.
            CreateMap<Account, AccountResponse>()
                //.ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.Name != null ? src.Role.Name : null));
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.Name));
            CreateMap<Account, AuthenticateResponse>();
            CreateMap<RegisterRequest, Account>();
            CreateMap<CreateAccountRequest, Account>();
            CreateMap<UpdateAccountRequest, Account>()
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
            //  - Configures the mappings for EmailTemplate-related objects.
            CreateMap<EmailTemplate, EmailTemplateResponse>();
            CreateMap<EmailTemplate, EmailTemplateNonDynamicResponse>();
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
            //  - Configures the mappings for Section-related objects.
            CreateMap<Section, SectionResponse>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.ContentType))
                .ForMember(dest => dest.Layout, opt => opt.MapFrom(src => src.Layout));
            CreateMap<Section, CreateSectionWithAdditonalRequest>();

            CreateMap<CreateSectionRequest, Section>();
            CreateMap<CreateSectionWithAdditonalRequest, Section>();
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
