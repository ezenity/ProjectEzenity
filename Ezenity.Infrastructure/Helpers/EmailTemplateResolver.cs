using Ezenity.Application.Abstractions.Configuration;
using Ezenity.Application.Abstractions.Emails;
using Microsoft.Extensions.Hosting;

namespace Ezenity.Infrastructure.Helpers
{
    /// <summary>
    /// Resolves the Razor view path for an email template.
    ///
    /// IMPORTANT:
    /// - ICompositeViewEngine resolves *virtual* paths (~/Views/.. or /Views/..),
    ///   not physical disk paths like /srv/.../file.cshtml.
    /// - Since your templates live under Ezenity.RazorViews/Views/*,
    ///   we return a virtual path under /Views/EmailTemplates.
    /// </summary>
    public class EmailTemplateResolver : IEmailTemplateResolver
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly IAppSettings _appSettings;

        public EmailTemplateResolver(IHostEnvironment hostEnvironment, IAppSettings appSettings)
        {
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        /// <summary>
        /// Returns a Razor view *virtual* path for the template.
        /// Example: templateName = "EmailVerification"
        /// Returns: "~/Views/EmailTemplates/EmailVerification.cshtml"
        /// </summary>
        public string GetTemplatePath(string templateName)
        {
            if (string.IsNullOrWhiteSpace(templateName))
                throw new ArgumentException("Template name cannot be null/empty.", nameof(templateName));

            // Normalize input just in case someone passes "EmailVerification.cshtml"
            templateName = templateName.Trim();

            if (templateName.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase))
                templateName = templateName[..^".cshtml".Length];

            // Your new location (RazorViews project):
            return $"~/Views/EmailTemplates/{templateName}.cshtml";
        }
    }
}
