using Ezenity.Core.Interfaces;
using Ezenity.Infrastructure.Helpers;
using Microsoft.Extensions.Configuration;
using System;

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
        /// <returns>A populated IAppSettings object.</returns>
        public static IAppSettings Create(IConfiguration configuration)
        {
            var deploymentEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var isProduction = deploymentEnvironment == "Production";

            var secretKey = isProduction
                            ? Environment.GetEnvironmentVariable("EZENITY_SECRET_KEY") ?? throw new InvalidOperationException("Secret key must be provided in environment variables for Production.")
                            : configuration["AppSettings:Secret"];

            var baseUrl = isProduction
                          ? Environment.GetEnvironmentVariable("EZENITY_BASE_URL") ?? throw new InvalidOperationException("Base URL must be provided in environment variables for Production.")
                          : configuration["AppSettings:BaseUrl"];

            var refreshTokenTTL = int.Parse(isProduction
                                            ? Environment.GetEnvironmentVariable("EZENITY_REFRESH_TOKEN_TTL") ?? configuration["AppSettings:RefreshTokenTTL"]
                                            : configuration["AppSettings:RefreshTokenTTL"]);

            var emailFrom = isProduction
                            ? Environment.GetEnvironmentVariable("EZENITY_EMAIL_FROM") ?? configuration["AppSettings:EmailFrom"]
                            : configuration["AppSettings:EmailFrom"];

            var smtpHost = isProduction
                            ? Environment.GetEnvironmentVariable("EZENITY_SMTP_HOST") ?? configuration["AppSettings:SmtpHost"]
                            : configuration["AppSettings:SmtpHost"];

            var smtpPort = int.Parse(isProduction
                                     ? Environment.GetEnvironmentVariable("EZENITY_SMTP_PORT") ?? configuration["AppSettings:SmtpPort"]
                                     : configuration["AppSettings:SmtpPort"]);

            var smtpUser = isProduction
                           ? Environment.GetEnvironmentVariable("EZENITY_SMTP_USER") ?? configuration["AppSettings:SmtpUser"]
                           : configuration["AppSettings:SmtpUser"];

            var smtpPass = isProduction
                           ? Environment.GetEnvironmentVariable("EZENITY_SMTP_PASSWORD") ?? configuration["AppSettings:SmtpPass"]
                           : configuration["AppSettings:SmtpPass"];

            var smtpEnabledSsl = bool.Parse(isProduction
                                            ? Environment.GetEnvironmentVariable("EZENITY_SMTP_ENABLED_SSL") ?? configuration["AppSettings:SmtpEnabledSsl"]
                                            : configuration["AppSettings:SmtpEnabledSsl"]);

            var accessToken = isProduction
                              ? Environment.GetEnvironmentVariable("EZENITY_ACCESS_TOKEN") ?? configuration["AppSettings:AccessToken"]
                              : configuration["AppSettings:AccessToken"];

            var emailMessageIdDomain = isProduction
                                        ? Environment.GetEnvironmentVariable("EZENITY_EMAIL_MESSAGE_ID_DOMAIN") ?? configuration["AppSettings:EmailMessageIdDomain"]
                                        : configuration["AppSettings:EmailMessageIdDomain"];

            var emailTemplateBasePath = isProduction
                                        ? Environment.GetEnvironmentVariable("EZENITY_EMAIL_TEMPLATE_BASE_PATH") ?? configuration["AppSettings:EmailTemplateBasePath"]
                                        : configuration["AppSettings:EmailTemplateBasePath"];

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
                    smtpEnabledSsl,
                    accessToken,
                    emailMessageIdDomain,
                    emailTemplateBasePath
                    )
                );
        }
    }
}
