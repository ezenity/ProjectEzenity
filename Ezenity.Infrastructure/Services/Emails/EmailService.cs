using Ezenity.Core.Entities.Emails;
using Ezenity.Core.Helpers.Exceptions;
using Ezenity.Core.Interfaces; // keep this ONLY if IDataContext is actually in here
using Ezenity.Core.Services.Common; // keep ONLY if your IEmailTemplateService is here
using Ezenity.Core.Services.Emails; // <-- IMPORTANT: this is where IEmailService should live after you move it
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ezenity.Infrastructure.Services.Emails
{
    /// <summary>
    /// Infrastructure implementation of IEmailService.
    /// This class is responsible for sending email using SMTP.
    ///
    /// Core idea:
    /// - Core defines "what" (IEmailService).
    /// - Infrastructure defines "how" (SMTP/MailKit).
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IAppSettings _appSettings;
        private readonly IDataContext _context; // you may not need this anymore, but keeping it since you had it
        private readonly IWebHostEnvironment _env;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly ILogger<AccountService> _logger;

        public EmailService(
            IAppSettings appSettings,
            IDataContext context,
            IWebHostEnvironment env,
            IEmailTemplateService emailTemplateService,
            ILogger<AccountService> logger)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _emailTemplateService = emailTemplateService ?? throw new ArgumentNullException(nameof(emailTemplateService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Sends an email asynchronously based on an EmailMessage and a template.
        /// </summary>
        public async Task SendEmailAsync(EmailMessage message)
        {
            // 1) Validate inputs early (fail fast with meaningful messages)
            if (message == null)
                throw new AppException("Email message cannot be null.");

            if (string.IsNullOrWhiteSpace(message.To))
                throw new AppException("Email 'To' address is required.");

            if (string.IsNullOrWhiteSpace(message.TemplateName))
                throw new AppException("Email 'TemplateName' is required.");

            // Ensure DynamicValues is never null to avoid null-ref issues
            message.DynamicValues ??= new Dictionary<string, string>();

            // 2) Pull template from DB/service layer (your _emailTemplateService)
            var emailTemplate = await _emailTemplateService.GetByNameAsync(message.TemplateName);
            if (emailTemplate == null)
                throw new AppException($"Email template '{message.TemplateName}' not found.");

            // 3) Prepare placeholder values (merge template defaults + request dynamic values)
            //    - If your template already has PlaceholderValues, we keep them
            //    - Then overwrite/add with message.DynamicValues
            emailTemplate.PlaceholderValues ??= new Dictionary<string, string>();

            foreach (var kv in message.DynamicValues)
                emailTemplate.PlaceholderValues[kv.Key] = kv.Value;

            // 4) Resolve nested placeholders: if {A} contains "{B}" it will be replaced
            //    Example: A = "Hello {FirstName}" and FirstName="Anthony"
            emailTemplate.PlaceholderValues = ResolveNestedPlaceholders(emailTemplate.PlaceholderValues);

            // 5) Render body
            //    - If ContentViewPath exists, your email template service should render Razor
            //    - Otherwise you can fall back to raw TemplateBody + replace placeholders
            string bodyHtml;

            if (!string.IsNullOrWhiteSpace(emailTemplate.ContentViewPath))
            {
                // Renders Razor view based on your implementation
                bodyHtml = await _emailTemplateService.RenderEmailTemplateAsync(
                    message.TemplateName,
                    emailTemplate.PlaceholderValues
                );
            }
            else
            {
                // Fallback: use TemplateBody and replace placeholders manually (if you store it)
                // If your EmailTemplate entity doesn't have TemplateBody anymore, adjust/remove this.
                //var raw = emailTemplate.TemplateBody ?? string.Empty;
                //bodyHtml = ReplacePlaceholders(raw, emailTemplate.PlaceholderValues);

                // If ContentViewPath is missing, this template is misconfigured in DB.
                // Fail fast instead of silently sending empty emails.
                throw new AppException(
                    $"Email template '{message.TemplateName}' is missing ContentViewPath. " +
                    "Update the template record to point to a .cshtml view."
                );
            }

            // Subject from template wins (unless you intentionally want message.Subject override)
            message.Subject = emailTemplate.Subject ?? message.Subject ?? "(no subject)";

            // 6) Build the actual MimeMessage that MailKit sends
            var mime = BuildMimeMessage(message, bodyHtml);

            // 7) Send via SMTP
            //    IMPORTANT:
            //    - Port 465 typically uses SSL-on-connect
            //    - Port 587 typically uses STARTTLS
            //    - Port 25 often blocked on VPS providers and/or requires STARTTLS
            await SendViaSmtpAsync(mime);
        }

        /// <summary>
        /// Build the email with From/To/Subject/Body.
        /// </summary>
        private MimeMessage BuildMimeMessage(EmailMessage message, string bodyHtml)
        {
            var mime = new MimeMessage();

            // Custom MessageId is optional but useful for tracking
            var messageIdDomain = string.IsNullOrWhiteSpace(_appSettings.EmailMessageIdDomain)
                ? "localhost"
                : _appSettings.EmailMessageIdDomain;

            mime.MessageId = $"{Guid.NewGuid()}@{messageIdDomain}";

            // FROM:
            // In dev you may want forced "noreply" to avoid your provider blocking spoofing
            // In prod you should normally send FROM your authenticated mailbox (SMTP user)
            // Many SMTP providers will reject if From is not allowed.
            var fromAddress = GetFromAddress(message);

            mime.From.Add(MailboxAddress.Parse(fromAddress));

            // TO:
            mime.To.Add(MailboxAddress.Parse(message.To));

            // SUBJECT:
            mime.Subject = message.Subject ?? "(no subject)";

            // BODY:
            mime.Body = new TextPart(TextFormat.Html)
            {
                Text = bodyHtml ?? string.Empty
            };

            return mime;
        }

        /// <summary>
        /// Decide what From address to use.
        /// Most providers require the From address to match the authenticated SMTP account.
        /// </summary>
        private string GetFromAddress(EmailMessage message)
        {
            // Prefer configured SMTP user as the sender (most compatible)
            // If you have app setting like EmailFrom, you can use that instead.
            var smtpUser = _appSettings.SmtpUser;

            // If you want a "display name", it should be set like: "Ezenity <noreply@ezenity.com>"
            // MailboxAddress.Parse supports that format.
            if (_env.IsDevelopment())
            {
                // In dev, force to SMTP user to avoid provider rejection
                return smtpUser;
            }

            // In prod, still safest to use smtpUser unless you KNOW your provider allows aliases
            // If message.From is set and allowed, you can prefer it:
            if (!string.IsNullOrWhiteSpace(message.From))
                return message.From;

            return smtpUser;
        }

        /// <summary>
        /// Actually connects/authenticates/sends using SMTP (MailKit).
        /// </summary>
        private async Task SendViaSmtpAsync(MimeMessage mime)
        {
            // Basic config validation
            if (string.IsNullOrWhiteSpace(_appSettings.SmtpHost))
                throw new AppException("SMTP host is not configured (SmtpHost).");

            if (_appSettings.SmtpPort <= 0)
                throw new AppException("SMTP port is not configured (SmtpPort).");

            if (string.IsNullOrWhiteSpace(_appSettings.SmtpUser))
                throw new AppException("SMTP username is not configured (SmtpUser).");

            if (string.IsNullOrWhiteSpace(_appSettings.SmtpPass))
                throw new AppException("SMTP password is not configured (SmtpPass).");

            using var client = new SmtpClient();

            try
            {
                // NOTE:
                // Do NOT do ServerCertificateValidationCallback = always true in production.
                // That disables TLS security.
                //
                // If you need it temporarily for debugging, you can enable it only in development.
                if (_env.IsDevelopment())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                }

                // Choose TLS mode based on port (common conventions)
                // 465 = SSL-on-connect
                // 587 = STARTTLS
                // Otherwise try StartTlsWhenAvailable (safer than no TLS)
                SecureSocketOptions socketOptions =
                    _appSettings.SmtpPort == 465 ? SecureSocketOptions.SslOnConnect :
                    _appSettings.SmtpPort == 587 ? SecureSocketOptions.StartTls :
                    SecureSocketOptions.StartTlsWhenAvailable;

                // CONNECT
                await client.ConnectAsync(_appSettings.SmtpHost, _appSettings.SmtpPort, socketOptions);

                // AUTH
                // Don’t log your password. Ever.
                await client.AuthenticateAsync(_appSettings.SmtpUser, _appSettings.SmtpPass);

                // SEND
                await client.SendAsync(mime);

                // DISCONNECT
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // This message is what bubbles up to your API response right now.
                // Keep it clear and keep the original exception as InnerException.
                //throw new AppException("SMTP send failed. Check SMTP host/port/TLS and credentials.", ex);

                // Log the REAL error with full detail (type + message + inner)
                // If you don't have _logger injected yet, at least Console.WriteLine(ex.ToString());
                // Better: inject ILogger<EmailService> and use it here.
                _logger.LogError(ex, "SMTP send failed. Host={Host} Port={Port} User={User}",
                     _appSettings.SmtpHost, _appSettings.SmtpPort, _appSettings.SmtpUser);

                // Temporarily surface the real error message in the thrown exception (DEV ONLY).
                throw new AppException(
                    $"SMTP send failed: {ex.GetType().Name} - {ex.Message}",
                    ex
                );
            }
        }

        /// <summary>
        /// Replaces {Placeholders} in a template with values.
        /// </summary>
        private static string ReplacePlaceholders(string template, IDictionary<string, string> values)
        {
            var result = template ?? string.Empty;

            if (values == null || values.Count == 0)
                return result;

            foreach (var kv in values)
            {
                var placeholder = "{" + kv.Key + "}";
                result = result.Replace(placeholder, kv.Value ?? string.Empty);
            }

            return result;
        }

        /// <summary>
        /// If placeholder values themselves contain placeholders, resolve them.
        /// This prevents issues where one placeholder depends on another.
        /// </summary>
        private static Dictionary<string, string> ResolveNestedPlaceholders(IDictionary<string, string> values)
        {
            var resolved = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (values == null)
                return resolved;

            // Copy first
            foreach (var kv in values)
                resolved[kv.Key] = kv.Value ?? string.Empty;

            // Resolve nested placeholders with a few passes (prevents infinite loops)
            // Example:
            //   Greeting = "Hello {FirstName}"
            //   FirstName = "Anthony"
            const int maxPasses = 5;

            for (int pass = 0; pass < maxPasses; pass++)
            {
                var changed = false;

                foreach (var key in new List<string>(resolved.Keys))
                {
                    var current = resolved[key];

                    foreach (var inner in resolved)
                    {
                        var token = "{" + inner.Key + "}";

                        if (!string.IsNullOrEmpty(current) && current.Contains(token))
                        {
                            current = current.Replace(token, inner.Value ?? string.Empty);
                            changed = true;
                        }
                    }

                    resolved[key] = current;
                }

                if (!changed)
                    break;
            }

            return resolved;
        }
    }
}
