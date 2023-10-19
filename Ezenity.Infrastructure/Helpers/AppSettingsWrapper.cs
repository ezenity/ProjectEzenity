using Ezenity.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace Ezenity.Infrastructure.Helpers
{
    public class AppSettingsWrapper : IAppSettings
    {
        private readonly IAppSettings _appSettings;

        public AppSettingsWrapper(IAppSettings appSettings)
        {
            _appSettings = appSettings ?? throw new ArgumentException(nameof(appSettings));
        }

        public string BaseUrl => _appSettings.BaseUrl;
        public string Secret => _appSettings.Secret;
        public int RefreshTokenTTL => _appSettings.RefreshTokenTTL;
        public string EmailFrom => _appSettings.EmailFrom;
        public string SmtpHost => _appSettings.SmtpHost;
        public int SmtpPort => _appSettings.SmtpPort;
        public string SmtpUser => _appSettings.SmtpUser;
        public string SmtpPass => _appSettings.SmtpPass;
        public bool SmtpEnableSsl => _appSettings.SmtpEnableSsl;
        public string AccessToken => _appSettings.AccessToken;
    }
}
