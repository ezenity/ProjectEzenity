using Ezenity.Core.Services.Emails;
using Ezenity.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using System;

namespace Ezenity.Infrastructure.Helpers
{
    public class EmailTemplateResolver : IEmailTemplateResolver
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly IAppSettings _appSettings;

        public EmailTemplateResolver(IHostEnvironment hostEnvironment, IAppSettings appSettings)
        {
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public string GetTemplatePath(string templateName)
        {
            if (string.IsNullOrWhiteSpace(templateName))
                throw new ArgumentException("Template name cannot be null/empty.", nameof(templateName));

            // Allow templateName to be passed with or without ".cshtml"
            var fileName = templateName.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase)
                ? templateName
                : templateName + ".cshtml";

            // Example values you can use:
            // "Areas/EmailTemplates/Views"  (if templates live under Ezenity.API/Areas/EmailTemplates/Views)
            // "Views"                      (if templates live under Ezenity.RazorViews/Views)
            var baseVirtual = _appSettings.EmailTemplateBasePath;

            if (string.IsNullOrWhiteSpace(baseVirtual))
                baseVirtual = "Views";
                //baseVirtual = "Areas/EmailTemplates/Views";

            // Normalize to a clean virtual path segment
            baseVirtual = baseVirtual
                .Replace('\\', '/')
                .Trim()
                .TrimStart('~')
                .TrimStart('/')
                .TrimEnd('/');

            // MVC view engine expects app-relative paths like "~/{...}"
            return $"~/{baseVirtual}/{fileName}";
        }
    }
}
