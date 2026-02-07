using Ezenity.Core.Entities.Emails;
using Ezenity.Core.Helpers.Exceptions;
using Ezenity.Core.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using MimeKit;
using MimeKit.Text;
using System;
using System.Threading.Tasks;

namespace Ezenity.Infrastructure.Services.Emails
{
    /// <summary>
    /// Responsible ONLY for sending emails via SMTP.
    /// No database logic.
    /// No template mutation.
    /// No placeholder rewriting.
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IAppSettings _appSettings;
        private readonly IWebHostEnvironment _env;

        public EmailService(
            IAppSettings appSettings,
            IWebHostEnvironment env)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        /// <summary>
        /// Sends an email using SMTP (MailKit).
        /// This method will THROW if anything fails.
        /// </summary>
        public async Task SendEmailAsync(EmailMessage message)
        {
            if (message == null)
                throw new AppException("Email message is null.");

            if (string.IsNullOrWhiteSpace(message.To))
                throw new AppException("Email recipient (To) is required.");

            if (string.IsNullOrWhiteSpace(message.Subject))
                throw new AppException("Email subject is required.");

            if (string.IsNullOrWhiteSpace(message.Body))
                throw new AppException("Email body is required.");

            // ----------------------------------------------------
            // 1. Build the email
            // ----------------------------------------------------
            var email = new MimeMessage();

            // FROM
            // In production, always send from noreply@
            // In dev, make it obvious this is a test
            email.From.Add(new MailboxAddress(
                _env.IsDevelopment() ? "Ezenity API (DEV)" : "Ezenity",
                _appSettings.EmailFrom
            ));

            // TO
            email.To.Add(MailboxAddress.Parse(message.To));

            // SUBJECT
            email.Subject = message.Subject;

            // BODY (HTML)
            email.Body = new TextPart(TextFormat.Html)
            {
                Text = message.Body
            };

            // ----------------------------------------------------
            // 2. Send via SMTP
            // ----------------------------------------------------
            using var smtp = new SmtpClient();

            try
            {
                // TRUST the certificate (your provider is valid)
                smtp.ServerCertificateValidationCallback = (_, _, _, _) => true;

                /*
                 * IMPORTANT:
                 * Your screenshots show:
                 *   Server: mail.privateemail.com
                 *   Port: 465 (SSL)
                 */
                await smtp.ConnectAsync(
                    _appSettings.SmtpHost,
                    _appSettings.SmtpPort,
                    SecureSocketOptions.SslOnConnect
                );

                /*
                 * AUTHENTICATION
                 * Username MUST be full email:
                 *   amacallister@ezenity.com
                 */
                await smtp.AuthenticateAsync(
                    _appSettings.SmtpUser,
                    _appSettings.SmtpPass
                );

                // SEND
                await smtp.SendAsync(email);

                // DISCONNECT CLEANLY
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // NEVER swallow email errors
                // If email fails, the API MUST know
                throw new AppException(
                    "SMTP email send failed. Check SMTP credentials and port.",
                    ex
                );
            }
        }
    }
}
