using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using Ezenity.Core.Entities.Emails;
using Ezenity.Core.Helpers.Exceptions;
using Ezenity.Core.Interfaces;
using Ezenity.Core.Services.Common;

namespace Ezenity.Infrastructure.Services.Emails
{
    /// <summary>
    /// Sends application emails using SMTP (MailKit).
    ///
    /// Key goals:
    /// - Fail fast if SMTP config is missing or invalid
    /// - Render a template (Razor or stored template content) and inject placeholders
    /// - Connect/auth/send with safe TLS defaults
    /// - Never log secrets (SMTP password)
    /// - Preserve the real exception for debugging
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IAppSettings _appSettings;
        private readonly IWebHostEnvironment _env;
        private readonly IEmailTemplateService _emailTemplateService;

        public EmailService(
            IAppSettings appSettings,
            IDataContext context, // still accepted because your constructor had it; not used here directly
            IWebHostEnvironment env,
            IEmailTemplateService emailTemplateService)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _emailTemplateService = emailTemplateService ?? throw new ArgumentNullException(nameof(emailTemplateService));
        }

        /// <summary>
        /// Primary entry point: build, render, and send an email.
        /// </summary>
        public async Task SendEmailAsync(EmailMessage message)
        {
            // -----------------------------
            // 1) Basic validation (fail fast)
            // -----------------------------
            if (message == null)
                throw new AppException("Email message is required.");

            if (string.IsNullOrWhiteSpace(message.To))
                throw new AppException("Email 'To' is required.");

            if (string.IsNullOrWhiteSpace(message.TemplateName))
                throw new AppException("Email 'TemplateName' is required.");

            ValidateSmtpConfiguration();

            // ----------------------------------------------------
            // 2) Get template from DB/service and build placeholders
            // ----------------------------------------------------
            // Your previous code was overwriting template placeholders in a confusing way.
            // Here we treat message.DynamicValues as the source of truth.
            var template = await _emailTemplateService.GetByNameAsync(message.TemplateName);
            if (template == null)
                throw new AppException($"Email template '{message.TemplateName}' not found.");

            // Ensure we always have a dictionary to work with.
            var placeholders = message.DynamicValues ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Optional: allow template to also have default placeholder values; message overrides them.
            // If your template.PlaceholderValues is populated, merge them.
            if (template.PlaceholderValues != null && template.PlaceholderValues.Count > 0)
            {
                // Start from template defaults, then overwrite with message values.
                var merged = new Dictionary<string, string>(template.PlaceholderValues, StringComparer.OrdinalIgnoreCase);
                foreach (var kv in placeholders)
                    merged[kv.Key] = kv.Value ?? string.Empty;

                placeholders = merged;
            }

            // ----------------------------------------------------
            // 3) Render the template body
            // ----------------------------------------------------
            // If ContentViewPath exists, we assume your template service can render it.
            // Otherwise, we fallback to raw TemplateBody (if you have that field).
            string bodyHtml;

            if (!string.IsNullOrWhiteSpace(template.ContentViewPath))
            {
                // Render Razor/CSHTML (or whatever your template service does)
                bodyHtml = await _emailTemplateService.RenderEmailTemplateAsync(template.TemplateName, placeholders);
            }
            else
            {
                // Fallback: if you store raw HTML in DB, use it.
                // If template.TemplateBody doesn't exist in your model, replace this with the correct field.
                var raw = template.TemplateBody ?? string.Empty;

                // Replace {Key} placeholders in raw HTML
                bodyHtml = ApplyPlaceholders(raw, placeholders);
            }

            // Subject: use template subject by default; allow message.Subject override if you want.
            var subject = !string.IsNullOrWhiteSpace(message.Subject)
                ? message.Subject
                : (template.Subject ?? string.Empty);

            // ----------------------------------------------------
            // 4) Build MimeMessage
            // ----------------------------------------------------
            var mime = BuildMimeMessage(message, subject, bodyHtml);

            // ----------------------------------------------------
            // 5) Send via SMTP
            // ----------------------------------------------------
            try
            {
                using var smtp = new SmtpClient();

                // IMPORTANT:
                // - In PROD, do NOT accept invalid certificates.
                // - In DEV, you *may* relax cert validation to avoid local/self-signed issues.
                if (_env.IsDevelopment())
                {
                    smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
                }

                // Choose TLS behavior:
                // - If your SMTP server expects STARTTLS, use StartTlsWhenAvailable
                // - If it expects implicit TLS (465), use SslOnConnect
                // If you have an appSetting like SmtpEnableSsl or SmtpUseStartTls,
                // wire it here. For now we use StartTlsWhenAvailable (safe default).
                var secureOption = SecureSocketOptions.StartTlsWhenAvailable;

                await smtp.ConnectAsync(_appSettings.SmtpHost, _appSettings.SmtpPort, secureOption).ConfigureAwait(false);

                // Authenticate only if username is provided.
                // Some SMTP relays allow anonymous sending from localhost.
                if (!string.IsNullOrWhiteSpace(_appSettings.SmtpUser))
                {
                    await smtp.AuthenticateAsync(_appSettings.SmtpUser, _appSettings.SmtpPass).ConfigureAwait(false);
                }

                await smtp.SendAsync(mime).ConfigureAwait(false);
                await smtp.DisconnectAsync(true).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Don’t swallow the root cause. Wrap it but keep ex as InnerException.
                // This is the error you’re currently seeing: "An unexpected error occurred while sending the email."
                throw new AppException("An unexpected error occurred while sending the email.", ex);
            }
        }

        // -----------------------------
        // Helper: Validate SMTP settings
        // -----------------------------
        private void ValidateSmtpConfiguration()
        {
            if (string.IsNullOrWhiteSpace(_appSettings.SmtpHost))
                throw new AppException("SMTP host is not configured (SmtpHost).");

            if (_appSettings.SmtpPort <= 0)
                throw new AppException("SMTP port is not configured or invalid (SmtpPort).");

            // If you require authentication, also ensure user/pass exists.
            // If you sometimes allow anonymous, keep this lenient.
            // Uncomment if you ALWAYS require auth:
            //
            // if (string.IsNullOrWhiteSpace(_appSettings.SmtpUser) || string.IsNullOrWhiteSpace(_appSettings.SmtpPass))
            //     throw new AppException("SMTP credentials are not configured (SmtpUser/SmtpPass).");

            if (string.IsNullOrWhiteSpace(_appSettings.EmailMessageIdDomain))
            {
                // Not strictly required, but helps.
                // Use something stable like "ezenity.com".
                // We'll allow it, but Message-Id will fall back.
            }
        }

        // ----------------------------------------
        // Helper: Create the actual outgoing message
        // ----------------------------------------
        private MimeMessage BuildMimeMessage(EmailMessage message, string subject, string bodyHtml)
        {
            var email = new MimeMessage();

            // Message-Id
            var domain = string.IsNullOrWhiteSpace(_appSettings.EmailMessageIdDomain)
                ? "local"
                : _appSettings.EmailMessageIdDomain.Trim();

            email.MessageId = $"{Guid.NewGuid()}@{domain}";

            // FROM
            // In development, it’s common to use a stable noreply so you don’t get rejected by relays.
            // In production, you should send from a verified sender/domain that your SMTP allows.
            if (_env.IsDevelopment())
            {
                // If you have a configured From address in settings, use it here.
                email.From.Add(MailboxAddress.Parse("noreply@ezenity.com"));
            }
            else
            {
                // If message.From is empty, you can fallback to a configured "From" in app settings (recommended).
                // If message.From contains a name/email, adjust parsing accordingly.
                var fromAddress = !string.IsNullOrWhiteSpace(message.From)
                    ? message.From
                    : "noreply@ezenity.com";

                email.From.Add(MailboxAddress.Parse(fromAddress));
            }

            // TO
            // This expects a single email. If you support multiple recipients, split by comma.
            email.To.Add(MailboxAddress.Parse(message.To));

            // SUBJECT
            email.Subject = subject ?? string.Empty;

            // BODY (HTML)
            email.Body = new TextPart(TextFormat.Html)
            {
                Text = bodyHtml ?? string.Empty
            };

            return email;
        }

        // ----------------------------------------
        // Helper: Replace {Key} placeholders safely
        // ----------------------------------------
        private static string ApplyPlaceholders(string input, IDictionary<string, string> placeholders)
        {
            if (string.IsNullOrEmpty(input) || placeholders == null || placeholders.Count == 0)
                return input ?? string.Empty;

            var output = input;

            // Simple placeholder style: {FirstName}, {VerifyUrl}, etc.
            foreach (var kv in placeholders)
            {
                var key = kv.Key?.Trim();
                if (string.IsNullOrEmpty(key))
                    continue;

                var token = "{" + key + "}";
                var value = kv.Value ?? string.Empty;

                // Replace all occurrences
                output = output.Replace(token, value, StringComparison.OrdinalIgnoreCase);
            }

            return output;
        }
    }
}
