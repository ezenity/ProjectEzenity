using Ezenity.Core.Services.Emails;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
      string basePath = AppDomain.CurrentDomain.BaseDirectory;

      if(_hostEnvironment.IsDevelopment())
      {
        basePath = Path.Combine(basePath);
      }

      var templatePath = Path.Combine(basePath, _appSettings.EmailTemplateBasePath, $"{templateName}.cshtml");
      Console.WriteLine($"Computed Path: {templatePath}");
      return templatePath;
    }
  }
}
