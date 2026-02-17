using AutoMapper;
using Ezenity.Contracts.Models.Accounts;
using Ezenity.Contracts.Models.EmailTemplates;
using Ezenity.Contracts.Models.Sections;
using Ezenity.Domain.Entities.Accounts;
using Ezenity.Domain.Entities.EmailTemplates;
using static System.Collections.Specialized.BitVector32;

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
            CreateMap<Account, AuthenticateResponse>()
                .ForMember(dest => dest.JwtToken, opt => opt.Ignore()) // Handled by 'TokenHelper' class
                .ForMember(dest => dest.RefreshToken, opt => opt.Ignore()); // Handled by 'TokenHelper' class
            CreateMap<RegisterRequest, Account>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.RoleId, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.VerificationToken, opt => opt.Ignore())
                .ForMember(dest => dest.Verified, opt => opt.Ignore())
                .ForMember(dest => dest.ResetToken, opt => opt.Ignore())
                .ForMember(dest => dest.ResetTokenExpires, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordReset, opt => opt.Ignore())
                .ForMember(dest => dest.Created, opt => opt.Ignore())
                .ForMember(dest => dest.Updated, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore());
            CreateMap<CreateAccountRequest, Account>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.AcceptTerms, opt => opt.Ignore())
                .ForMember(dest => dest.RoleId, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => new Role {  Name = src.Role }))
                .ForMember(dest => dest.VerificationToken, opt => opt.Ignore())
                .ForMember(dest => dest.Verified, opt => opt.Ignore())
                .ForMember(dest => dest.ResetToken, opt => opt.Ignore())
                .ForMember(dest => dest.ResetTokenExpires, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordReset, opt => opt.Ignore())
                .ForMember(dest => dest.Created, opt => opt.Ignore())
                .ForMember(dest => dest.Updated, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore());
            CreateMap<UpdateAccountRequest, Account>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.AcceptTerms, opt => opt.Ignore())
                .ForMember(dest => dest.RoleId, opt => opt.Ignore())
                .ForMember(dest => dest.VerificationToken, opt => opt.Ignore())
                .ForMember(dest => dest.Verified, opt => opt.Ignore())
                .ForMember(dest => dest.ResetToken, opt => opt.Ignore())
                .ForMember(dest => dest.ResetTokenExpires, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordReset, opt => opt.Ignore())
                .ForMember(dest => dest.Created, opt => opt.Ignore())
                .ForMember(dest => dest.Updated, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.MapFrom<RoleResolver>());

            // Email Templates
            //  - Configures the mappings for EmailTemplate-related objects.
            CreateMap<EmailTemplate, EmailTemplateResponse>();
            CreateMap<CreateEmailTemplateRequest, EmailTemplate>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.PlaceholderValuesJson, opt => opt.Ignore());
            CreateMap<UpdateEmailTemplateRequest, EmailTemplate>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.PlaceholderValuesJson, opt => opt.Ignore())
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

            CreateMap<CreateSectionRequest, Section>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Created, opt => opt.Ignore())
                .ForMember(dest => dest.Updated, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.AccessLevel, opt => opt.Ignore())
                .ForMember(dest => dest.ParentSectionId, opt => opt.Ignore())
                .ForMember(dest => dest.MetaTags, opt => opt.Ignore());
            CreateMap<CreateSectionWithAdditonalRequest, Section>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Created, opt => opt.Ignore())
                .ForMember(dest => dest.Updated, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.ParentSectionId, opt => opt.Ignore());
            CreateMap<UpdateSectionRequest, Section>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Created, opt => opt.Ignore())
                .ForMember(dest => dest.Updated, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.AccessLevel, opt => opt.Ignore())
                .ForMember(dest => dest.ParentSectionId, opt => opt.Ignore())
                .ForMember(dest => dest.MetaTags, opt => opt.Ignore())
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
