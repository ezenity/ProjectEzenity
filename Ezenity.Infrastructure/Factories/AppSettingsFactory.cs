using Ezenity.Core.Interfaces;
using Ezenity.Infrastructure.Helpers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezenity.Infrastructure.Factories
{
    /// <summary>
    /// Factory class for creating AppSettings instances.
    /// </summary>
    public static class AppSettingsFactory
    {
        /// <summary>
        /// Creates an instance of IAppSettings based on provided IConfiguration and secret key.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="secretKey">The secret key.</param>
        /// <returns>A populated IAppSettings object.</returns>
        public static IAppSettings Create(IConfiguration configuration, string secretKey)
        {
            var baseUrl = configuration["AppSettings:BaseUrl"];
            var refreshTokenTTL = int.Parse(configuration["AppSettings:RefreshTokenTTL"]);
            var emailFrom = configuration["AppSettings:EmailFrom"];
            var smtpHost = configuration["AppSettings:SmtpHost"];
            var smtpPort = int.Parse(configuration["AppSettings:SmtpPort"]);
            var smtpUser = configuration["AppSettings:SmtpUser"];
            var smtpPass = configuration["AppSettings:SmtpPass"];
            var smtpEnableSsl = bool.Parse(configuration["AppSettings:SmtpEnableSsl"]);
            var accessToken = configuration["AppSettings:AccessToken"];

            return new AppSettingsWrapper(
                new AppSettings(
                    secretKey,
                    baseUrl,
                    refreshTokenTTL,
                    emailFrom,
                    smtpHost,
                    smtpPort,
                    smtpUser,
                    smtpPass,
                    smtpEnableSsl,
                    accessToken
                    )
                );
        }
    }
}
