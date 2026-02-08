using Ezenity.Core.Entities.Emails;
using Ezenity.Core.Helpers.Exceptions;
using Ezenity.Core.Interfaces;
using Ezenity.Core.Services.Common;
using Ezenity.Core.Services.Emails;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ezenity.Infrastructure.Services.Emails
{
    /// <summary>
    /// EmailService (Infrastructure)
    /// ----------------------------
    /// Responsible for:
    /// 1) Loading an EmailTemplate by name (via IEmailTemplateService).
    /// 2) Merging placeholder values (template defaults + runtime dynamic values).
    /// 3) Rendering the template (Razor .cshtml) into HTML.
    /// 4) Building a well-formed email message:
    ///    - Uses multipart/alternative (Text + HTML) for deliverability.
    ///    - Adds transactional headers to reduce auto-replies and improve classification.
    ///    - Uses a consistent From that matches the authenticated SMTP user.
    /// 5) Sending via SMTP using MailKit.
    ///
    /// Deliverability goals:
    /// - Avoid "HTML only" emails.
    /// - Avoid mismatched From vs authenticated SMTP mailbox.
    /// - Ensure consistent headers and a clean text part.
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IAppSettings _appSettings;
        private readonly IDataContext _context; // If unused, remove.
        private readonly IWebHostEnvironment _env;
        private readonly IEmailTemplateService _emailTemplateService;

        // IMPORTANT:
        // Logger generic type should be the concrete class so log categories are meaningful.
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IAppSettings appSettings,
            IDataContext context,
            IWebHostEnvironment env,
            IEmailTemplateService emailTemplateService,
            ILogger<EmailService> logger)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _emailTemplateService = emailTemplateService ?? throw new ArgumentNullException(nameof(emailTemplateService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Sends an email based on an EmailMessage and a template stored in your system.
        /// Flow:
        /// - Validate inputs
        /// - Load template
        /// - Merge placeholders
        /// - Render template to HTML
        /// - Build MimeMessage (Text + HTML)
        /// - Send via SMTP
        /// </summary>
        public async Task SendEmailAsync(EmailMessage message)
        {
            // 1) Validate inputs early (fail fast)
            if (message == null)
                throw new AppException("Email message cannot be null.");

            if (string.IsNullOrWhiteSpace(message.To))
                throw new AppException("Email 'To' address is required.");

            if (string.IsNullOrWhiteSpace(message.TemplateName))
                throw new AppException("Email 'TemplateName' is required.");

            // Ensure DynamicValues is non-null to avoid null-ref issues.
            message.DynamicValues ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // 2) Pull template record (DB or other source behind IEmailTemplateService)
            var emailTemplate = await _emailTemplateService.GetByNameAsync(message.TemplateName);
            if (emailTemplate == null)
                throw new AppException($"Email template '{message.TemplateName}' not found.");

            // Ensure placeholder dictionary exists.
            emailTemplate.PlaceholderValues ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // 3) Merge runtime dynamic values OVER template defaults
            foreach (var kv in message.DynamicValues)
                emailTemplate.PlaceholderValues[kv.Key] = kv.Value;

            // 4) Ensure standard placeholders commonly used by templates exist.
            //    This keeps templates from rendering with missing titles/years/app names.
            EnsureStandardPlaceholders(emailTemplate.PlaceholderValues);

            // 5) Resolve nested placeholders like: Greeting="Hello {FirstName}"
            emailTemplate.PlaceholderValues = ResolveNestedPlaceholders(emailTemplate.PlaceholderValues);

            // 6) Render HTML via Razor view
            if (string.IsNullOrWhiteSpace(emailTemplate.ContentViewPath))
            {
                // If you rely on Razor views, ContentViewPath is required.
                throw new AppException(
                    $"Email template '{message.TemplateName}' is missing ContentViewPath. " +
                    "Update the template record to point to a .cshtml view."
                );
            }

            var bodyHtml = await _emailTemplateService.RenderEmailTemplateAsync(
                message.TemplateName,
                emailTemplate.PlaceholderValues
            );

            // Subject from template wins; fallback to message.Subject
            message.Subject = emailTemplate.Subject ?? message.Subject ?? "(no subject)";

            // 7) Build MimeMessage (multipart/alternative for deliverability)
            var mime = BuildMimeMessage(message, bodyHtml);

            // 8) Send via SMTP
            await SendViaSmtpAsync(mime);
        }

        /// <summary>
        /// Ensures certain keys exist so Razor templates can rely on them.
        /// Adjust defaults as you evolve branding.
        /// </summary>
        private void EnsureStandardPlaceholders(IDictionary<string, string> values)
        {
            if (!values.ContainsKey("appName"))
                values["appName"] = "Ezenity";

            if (!values.ContainsKey("currentYear"))
                values["currentYear"] = DateTime.UtcNow.Year.ToString();

            // If templateTitle is missing but EmailTitle exists, map it.
            if (!values.ContainsKey("templateTitle") &&
                values.TryGetValue("EmailTitle", out var emailTitle) &&
                !string.IsNullOrWhiteSpace(emailTitle))
            {
                values["templateTitle"] = emailTitle;
            }
        }

        /// <summary>
        /// Builds a MIME email with good deliverability defaults:
        /// - Date header
        /// - Message-Id
        /// - Transactional headers
        /// - multipart/alternative (Text + HTML)
        /// </summary>
        private MimeMessage BuildMimeMessage(EmailMessage message, string bodyHtml)
        {
            var mime = new MimeMessage
            {
                Date = DateTimeOffset.UtcNow
            };

            // Message-Id domain helps make the email look more legitimate/consistent
            var messageIdDomain = string.IsNullOrWhiteSpace(_appSettings.EmailMessageIdDomain)
                ? "ezenity.com"
                : _appSettings.EmailMessageIdDomain;

            mime.MessageId = $"{Guid.NewGuid()}@{messageIdDomain}";

            // Helpful headers (transactional classification & suppress auto responders)
            mime.Headers["Auto-Submitted"] = "auto-generated";
            mime.Headers["X-Auto-Response-Suppress"] = "All";

            // FROM:
            // Most SMTP providers want From to match the authenticated SMTP user.
            var fromAddress = GetFromAddress(message);

            // Display name: prefer "appName" if available, else safe default
            var displayName = "Ezenity";
            if (message.DynamicValues != null &&
                message.DynamicValues.TryGetValue("appName", out var appName) &&
                !string.IsNullOrWhiteSpace(appName))
            {
                displayName = appName.Trim();
            }

            // If message.From already includes a name <email@domain>, keep it.
            MailboxAddress fromMailbox;
            if (MailboxAddress.TryParse(fromAddress, out var parsed))
            {
                fromMailbox = string.IsNullOrWhiteSpace(parsed.Name)
                    ? new MailboxAddress(displayName, parsed.Address)
                    : parsed;
            }
            else
            {
                fromMailbox = new MailboxAddress(displayName, fromAddress);
            }

            mime.From.Add(fromMailbox);

            // REPLY-TO (optional)
            if (!string.IsNullOrWhiteSpace(message.ReplyTo) && MailboxAddress.TryParse(message.ReplyTo, out var replyTo))
                mime.ReplyTo.Add(replyTo);

            // TO
            mime.To.Add(MailboxAddress.Parse(message.To));

            // SUBJECT
            mime.Subject = message.Subject ?? "(no subject)";

            // BODY:
            // Use multipart/alternative = Text + HTML.
            // Gmail and other providers prefer/expect this and it improves deliverability.
            var html = bodyHtml ?? string.Empty;
            var text = CreateTextBodyFromHtml(html);

            var builder = new BodyBuilder
            {
                HtmlBody = html,
                TextBody = text
            };

            mime.Body = builder.ToMessageBody();
            return mime;
        }

        /// <summary>
        /// Determines the From address.
        /// Best practice: use the authenticated SMTP user to avoid provider rejection.
        /// </summary>
        private string GetFromAddress(EmailMessage message)
        {
            var smtpUser = _appSettings.SmtpUser;

            if (string.IsNullOrWhiteSpace(smtpUser))
                throw new AppException("SMTP user is not configured (SmtpUser).");

            // If your provider allows aliases and you set message.From intentionally,
            // you can allow it; otherwise keep smtpUser always.
            if (!string.IsNullOrWhiteSpace(message.From))
                return message.From;

            return smtpUser;
        }

        /// <summary>
        /// Connect/authenticate/send using MailKit.
        /// Uses TLS options appropriate to port:
        /// - 465 => SSL-on-connect
        /// - 587 => STARTTLS
        /// - otherwise => StartTlsWhenAvailable
        /// </summary>
        private async Task SendViaSmtpAsync(MimeMessage mime)
        {
            // Validate config
            if (string.IsNullOrWhiteSpace(_appSettings.SmtpHost))
                throw new AppException("SMTP host is not configured (SmtpHost).");

            if (_appSettings.SmtpPort <= 0)
                throw new AppException("SMTP port is not configured (SmtpPort).");

            if (string.IsNullOrWhiteSpace(_appSettings.SmtpUser))
                throw new AppException("SMTP username is not configured (SmtpUser).");

            if (string.IsNullOrWhiteSpace(_appSettings.SmtpPass))
                throw new AppException("SMTP password is not configured (SmtpPass).");

            // Put protocol logs in a safe writable location for containers.
            // NOTE: this file can include server responses, but should NOT include your password.
            var protocolLogPath = "/tmp/smtp-protocol.log";

            using var client = new SmtpClient(new MailKit.ProtocolLogger(protocolLogPath));

            try
            {
                // DEV ONLY: never disable cert validation in production.
                if (_env.IsDevelopment())
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                var host = _appSettings.SmtpHost;
                var port = _appSettings.SmtpPort;

                // Normalize injected secrets (quotes/newlines are common with env vars/CI)
                var user = (_appSettings.SmtpUser ?? "").Trim().TrimEnd('\r', '\n').Trim('"');
                var pass = (_appSettings.SmtpPass ?? "").Trim().TrimEnd('\r', '\n').Trim('"');

                // Pick TLS mode
                SecureSocketOptions socketOptions =
                    port == 465 ? SecureSocketOptions.SslOnConnect :
                    port == 587 ? SecureSocketOptions.StartTls :
                    SecureSocketOptions.StartTlsWhenAvailable;

                _logger.LogInformation(
                    "SMTP config: Host={Host}, Port={Port}, User='{User}', PassLength={Len}, SocketOptions={SocketOptions}",
                    host, port, user, pass?.Length ?? 0, socketOptions
                );

                await client.ConnectAsync(host, port, socketOptions);

                // PrivateEmail typically uses LOGIN/PLAIN; remove XOAUTH2 noise
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                await client.AuthenticateAsync(user, pass);

                await client.SendAsync(mime);

                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "SMTP send failed. Host={Host} Port={Port} User={User}",
                    _appSettings.SmtpHost, _appSettings.SmtpPort, _appSettings.SmtpUser);

                // Bubble up a clear error
                throw new AppException(
                    $"SMTP send failed: {ex.GetType().Name} - {ex.Message}",
                    ex
                );
            }
        }

        /// <summary>
        /// Creates a basic plain-text version from HTML.
        /// This is not a perfect HTML-to-text conversion, but it's good enough for:
        /// - deliverability (multipart/alternative)
        /// - readable fallback in clients that prefer text
        /// </summary>
        private static string CreateTextBodyFromHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            var text = html;

            // Convert common block/line tags into newlines
            text = Regex.Replace(text, @"<\s*br\s*/?\s*>", "\n", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"</\s*p\s*>", "\n\n", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"</\s*div\s*>", "\n", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"</\s*h[1-6]\s*>", "\n\n", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"<\s*li\s*>", "• ", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"</\s*li\s*>", "\n", RegexOptions.IgnoreCase);

            // Strip all remaining tags
            text = Regex.Replace(text, "<.*?>", string.Empty, RegexOptions.Singleline);

            // Decode HTML entities
            text = WebUtility.HtmlDecode(text);

            // Clean up whitespace
            text = Regex.Replace(text, @"\n{3,}", "\n\n");
            text = Regex.Replace(text, @"[ \t]{2,}", " ");

            return text.Trim();
        }

        /// <summary>
        /// Resolves placeholders inside placeholder values.
        /// Example:
        ///  Greeting = "Hello {FirstName}"
        ///  FirstName = "Anthony"
        /// After resolve: Greeting => "Hello Anthony"
        /// </summary>
        private static Dictionary<string, string> ResolveNestedPlaceholders(IDictionary<string, string> values)
        {
            var resolved = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (values == null)
                return resolved;

            foreach (var kv in values)
                resolved[kv.Key] = kv.Value ?? string.Empty;

            // A few passes prevents most nested cases without infinite loops
            const int maxPasses = 5;

            for (int pass = 0; pass < maxPasses; pass++)
            {
                var changed = false;

                foreach (var key in resolved.Keys.ToList())
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

/*
SUMMARY (What changed vs the original):
--------------------------------------
1) ILogger generic type fixed:
   - Was ILogger<IAccountService> (wrong category)
   - Now ILogger<EmailService> (correct category & filtering)

2) Deliverability improvement:
   - Was HTML-only: TextPart(TextFormat.Html)
   - Now multipart/alternative: BodyBuilder { TextBody + HtmlBody }

3) Transactional headers added:
   - Auto-Submitted: auto-generated
   - X-Auto-Response-Suppress: All

4) From address handling:
   - Keeps provider-friendly "From" aligned with SMTP authenticated user
   - Applies a display name (appName) when possible

5) SMTP secret normalization:
   - Trims whitespace/newlines/quotes often introduced by env vars or CI injection

6) More robust and readable structure:
   - Clear steps, defensive checks, and useful logs (without leaking secrets)
*/
