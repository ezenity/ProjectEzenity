using AutoMapper;
using Ezenity.Contracts.Models.Accounts;
using Ezenity.Contracts.Models.EmailTemplates;
using Ezenity.Contracts.Models.Sections;
using Ezenity.Domain.Entities.Accounts;
using Ezenity.Domain.Entities.EmailTemplates;
using Ezenity.Domain.Entities.Sections;

namespace Ezenity.Application.Mapping;

/// <summary>
/// Defines the AutoMapper profiles for mapping between domain entities and model contract DTOs.
/// </summary>
public sealed class AutoMapperProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutoMapperProfile"/> class and configures object mappings.
    /// Mappings between model and entity objects
    /// </summary>
    public AutoMapperProfile()
    {
        ConfigureAccounts();
        ConfigureEmailTemplates();
        ConfigureSections();
    }

    /// <summary>
    //   Accounts
    //     - Configures the mappings for Account-related objects.
    /// </summary>
    private void ConfigureAccounts()
    {
        // Entity -> DTO
        CreateMap<Account, AccountResponse>()
            .ForMember(d => d.Role, opt => opt.MapFrom(s => s.Role != null ? s.Role.Name : null));

        CreateMap<Account, AuthenticateResponse>()
            .ForMember(d => d.JwtToken, opt => opt.Ignore()) // Handled by 'TokenHelper' class
            .ForMember(d => d.RefreshToken, opt => opt.Ignore()); // Handled by 'TokenHelper' class

        // DTO -> Entity (Create)
        CreateMap<RegisterRequest, Account>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.PasswordHash, opt => opt.Ignore())
            .ForMember(d => d.RoleId, opt => opt.Ignore())
            .ForMember(d => d.Role, opt => opt.Ignore())
            .ForMember(d => d.VerificationToken, opt => opt.Ignore())
            .ForMember(d => d.Verified, opt => opt.Ignore())
            .ForMember(d => d.ResetToken, opt => opt.Ignore())
            .ForMember(d => d.ResetTokenExpires, opt => opt.Ignore())
            .ForMember(d => d.PasswordReset, opt => opt.Ignore())
            .ForMember(d => d.Created, opt => opt.Ignore())
            .ForMember(d => d.Updated, opt => opt.Ignore())
            .ForMember(d => d.RefreshTokens, opt => opt.Ignore());

        CreateMap<CreateAccountRequest, Account>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.PasswordHash, opt => opt.Ignore())
            .ForMember(d => d.AcceptTerms, opt => opt.Ignore())
            .ForMember(d => d.RoleId, opt => opt.Ignore())
            .ForMember(d => d.Role, opt => opt.Ignore()) // let service/repo set role, don't create new Role in mapping
            .ForMember(d => d.VerificationToken, opt => opt.Ignore())
            .ForMember(d => d.Verified, opt => opt.Ignore())
            .ForMember(d => d.ResetToken, opt => opt.Ignore())
            .ForMember(d => d.ResetTokenExpires, opt => opt.Ignore())
            .ForMember(d => d.PasswordReset, opt => opt.Ignore())
            .ForMember(d => d.Created, opt => opt.Ignore())
            .ForMember(d => d.Updated, opt => opt.Ignore())
            .ForMember(d => d.RefreshTokens, opt => opt.Ignore());

        // DTO -> Entity (Update/Patch)
        CreateMap<UpdateAccountRequest, Account>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.PasswordHash, opt => opt.Ignore())
            .ForMember(d => d.AcceptTerms, opt => opt.Ignore())
            .ForMember(d => d.RoleId, opt => opt.Ignore())
            .ForMember(d => d.VerificationToken, opt => opt.Ignore())
            .ForMember(d => d.Verified, opt => opt.Ignore())
            .ForMember(d => d.ResetToken, opt => opt.Ignore())
            .ForMember(d => d.ResetTokenExpires, opt => opt.Ignore())
            .ForMember(d => d.PasswordReset, opt => opt.Ignore())
            .ForMember(d => d.Created, opt => opt.Ignore())
            .ForMember(d => d.Updated, opt => opt.Ignore())
            .ForMember(d => d.RefreshTokens, opt => opt.Ignore())
            //.ForMember(d => d.Role, opt => opt.MapFrom<RoleResolver>())
            .ForMember(d => d.Role, opt => opt.Ignore())
            .ForMember(d => d.RoleId, opt => opt.Ignore())
            .IgnoreNullOrEmptyStrings(); // key improvement
    }

    /// <summary>
    /// Email Templates
    ///  - Configures the mappings for EmailTemplate-related objects.
    /// </summary>
    private void ConfigureEmailTemplates()
    {
        CreateMap<EmailTemplate, EmailTemplateResponse>();

        CreateMap<CreateEmailTemplateRequest, EmailTemplate>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
            .ForMember(d => d.PlaceholderValuesJson, opt => opt.Ignore());

        CreateMap<UpdateEmailTemplateRequest, EmailTemplate>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
            .ForMember(d => d.PlaceholderValuesJson, opt => opt.Ignore())
            .IgnoreNullOrEmptyStrings();
    }

    /// <summary>
    /// Sections
    ///  - Configures the mappings for Section-related objects.
    /// </summary>
    private void ConfigureSections()
    {
        // Entity -> DTO (convention-based, no need to map each property)
        CreateMap<Section, SectionResponse>();

        // If you still want this for "edit screen prefill", it's fine:
        CreateMap<Section, CreateSectionWithAdditonalRequest>();

        // DTO -> Entity (Create)
        CreateMap<CreateSectionRequest, Section>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Created, opt => opt.Ignore())
            .ForMember(dest => dest.Updated, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.AccessLevel, opt => opt.Ignore())
            .ForMember(dest => dest.ParentSectionId, opt => opt.Ignore())
            .ForMember(dest => dest.MetaTags, opt => opt.Ignore());

        // Additional create inherits from CreateSectionRequest — so include base mapping
        CreateMap<CreateSectionWithAdditonalRequest, Section>()
            .IncludeBase<CreateSectionRequest, Section>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Created, opt => opt.Ignore())
            .ForMember(dest => dest.Updated, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.ParentSectionId, opt => opt.Ignore());

        // DTO -> Entity (Update/Patch)
        CreateMap<UpdateSectionRequest, Section>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Created, opt => opt.Ignore())
            .ForMember(dest => dest.Updated, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.AccessLevel, opt => opt.Ignore())
            .ForMember(dest => dest.ParentSectionId, opt => opt.Ignore())
            .ForMember(dest => dest.MetaTags, opt => opt.Ignore())

            // If we truly do NOT want content updated through UpdateSectionRequest, keep this:
            //.ForMember(dest => dest.Content, opt => opt.Ignore())

            .IgnoreNullOrEmptyStrings();
    }
}