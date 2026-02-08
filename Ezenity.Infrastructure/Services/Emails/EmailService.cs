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
    /// Responsibilities:
    /// 1) Load EmailTemplate by name (via IEmailTemplateService).
    /// 2) Merge placeholder values (template defaults + runtime dynamic values).
    /// 3) Render the template (Razor .cshtml) into HTML.
    /// 4) Build a well-formed email (multipart: Text + HTML).
    /// 5) Send via SMTP using MailKit.
    ///
    /// Notes on deliverability:
    /// - multipart/alternative (Text + HTML) is better than HTML-only.
    /// - "From" should ideally align with the authenticated SMTP mailbox (or an allowed alias).
    /// - Removing “template junk” (Litmus links, placeholder app name, double templates) helps reduce spam scoring.
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IAppSettings _appSettings;
        private readonly IDataContext _context; // If unused, remove from constructor + DI registration.
        private readonly IWebHostEnvironment _env;
        private readonly IEmailTemplateService _emailTemplateService;
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
        /// Sends an email using an EmailMessage and a stored template.
        /// Flow:
        /// - Validate
        /// - Load template
        /// - Merge placeholders
        /// - Render Razor => HTML
        /// - Build MimeMessage (Text + HTML)
        /// - Send via SMTP
        /// </summary>
        public async Task SendEmailAsync(EmailMessage message)
        {
            // 1) Validate input
            if (message == null)
                throw new AppException("Email message cannot be null.");

            if (string.IsNullOrWhiteSpace(message.To))
                throw new AppException("Email 'To' address is required.");

            if (string.IsNullOrWhiteSpace(message.TemplateName))
                throw new AppException("Email 'TemplateName' is required.");

            message.DynamicValues ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // 2) Load template
            var template = await _emailTemplateService.GetByNameAsync(message.TemplateName);
            if (template == null)
                throw new AppException($"Email template '{message.TemplateName}' not found.");

            if (string.IsNullOrWhiteSpace(template.ContentViewPath))
            {
                // Your pipeline uses Razor views, so ContentViewPath is required.
                throw new AppException(
                    $"Email template '{message.TemplateName}' is missing ContentViewPath. " +
                    "Update the template record to point to a .cshtml view."
                );
            }

            template.PlaceholderValues ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // 3) Merge runtime values over template defaults
            foreach (var kv in message.DynamicValues)
                template.PlaceholderValues[kv.Key] = kv.Value;

            // 4) Ensure common keys exist so the .cshtml does not KeyNotFound()
            EnsureStandardPlaceholders(template.PlaceholderValues);

            // 5) Resolve nested placeholders if you use {Tokens} inside values
            template.PlaceholderValues = ResolveNestedPlaceholders(template.PlaceholderValues);

            // 6) Render Razor => HTML
            var html = await _emailTemplateService.RenderEmailTemplateAsync(
                message.TemplateName,
                template.PlaceholderValues
            );

            // Subject preference: template.Subject > message.Subject > fallback
            message.Subject = template.Subject ?? message.Subject ?? "(no subject)";

            // 7) Build MIME message (From/To/Subject/Text+HTML)
            var mime = BuildMimeMessage(message, html);

            // 8) Send SMTP
            await SendViaSmtpAsync(mime);
        }

        /// <summary>
        /// Ensures the dictionary contains keys your templates expect.
        /// This prevents Razor from throwing KeyNotFoundException when using Model["key"].
        /// </summary>
        private static void EnsureStandardPlaceholders(IDictionary<string, string> values)
        {
            if (values == null) return;

            if (!values.ContainsKey("appName"))
                values["appName"] = "Ezenity";

            if (!values.ContainsKey("currentYear"))
                values["currentYear"] = DateTime.UtcNow.Year.ToString();

            if (!values.ContainsKey("templateTitle"))
            {
                if (values.TryGetValue("EmailTitle", out var t) && !string.IsNullOrWhiteSpace(t))
                    values["templateTitle"] = t;
                else
                    values["templateTitle"] = "Notification";
            }

            // Optional but useful for footers
            if (!values.ContainsKey("siteUrl"))
                values["siteUrl"] = "https://ezenity.com";

            if (!values.ContainsKey("supportEmail"))
                values["supportEmail"] = "support@ezenity.com";
        }

        /// <summary>
        /// Creates a MimeMessage with deliverability-friendly defaults:
        /// - Message-Id
        /// - Transactional headers
        /// - multipart/alternative (Text + HTML)
        /// - Sets From + Sender consistently
        /// </summary>
        private MimeMessage BuildMimeMessage(EmailMessage message, string bodyHtml)
        {
            var mime = new MimeMessage
            {
                Date = DateTimeOffset.UtcNow
            };

            // MessageId domain:
            // If your IAppSettings has EmailMessageIdDomain, use it.
            // Otherwise derive from From or fallback to "ezenity.com".
            var msgIdDomain = GetMessageIdDomain(message);
            mime.MessageId = $"{Guid.NewGuid()}@{msgIdDomain}";

            // Transactional headers (helps suppress auto-replies and classify as auto-generated)
            mime.Headers["Auto-Submitted"] = "auto-generated";
            mime.Headers["X-Auto-Response-Suppress"] = "All";

            // FROM:
            // Keep your message.From if provided, otherwise default to SMTP user (recommended).
            var fromValue = GetFromAddress(message);

            // Try parse "Name <email@domain>" format
            if (!MailboxAddress.TryParse(fromValue, out var fromMailbox))
                fromMailbox = new MailboxAddress("Ezenity", fromValue);

            mime.From.Add(fromMailbox);

            // Setting Sender can help when From is an alias (some systems like seeing it)
            mime.Sender = fromMailbox;

            // TO
            mime.To.Add(MailboxAddress.Parse(message.To));

            // SUBJECT
            mime.Subject = message.Subject ?? "(no subject)";

            // BODY: multipart/alternative (Text + HTML)
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
        /// Determines what "From" value to use.
        /// Best practice: use the authenticated SMTP user, unless you KNOW your provider allows your alias.
        /// </summary>
        private string GetFromAddress(EmailMessage message)
        {
            var smtpUser = _appSettings.SmtpUser;

            if (string.IsNullOrWhiteSpace(smtpUser))
                throw new AppException("SMTP user is not configured (SmtpUser).");

            // If caller set From, use it (but ensure your SMTP provider allows it)
            if (!string.IsNullOrWhiteSpace(message.From))
                return message.From;

            // Default: safest for SMTP acceptance
            return smtpUser;
        }

        /// <summary>
        /// Picks a stable Message-Id domain.
        /// Uses IAppSettings.EmailMessageIdDomain if present; otherwise derives from From/SMTP user.
        /// </summary>
        private string GetMessageIdDomain(EmailMessage message)
        {
            // If your IAppSettings DOES have this property, keep it.
            // If it DOES NOT, delete this block and always derive from From/SMTP user.
            var configured = _appSettings.EmailMessageIdDomain;
            if (!string.IsNullOrWhiteSpace(configured))
                return configured.Trim().Trim('.');

            var from = !string.IsNullOrWhiteSpace(message.From) ? message.From : _appSettings.SmtpUser;
            if (MailboxAddress.TryParse(from, out var mb) && mb.Address.Contains("@"))
                return mb.Address.Split('@')[1];

            return "ezenity.com";
        }

        /// <summary>
        /// Connects/authenticates/sends email with MailKit.
        /// Uses secure socket mode based on port:
        /// - 465 => SSL-on-connect
        /// - 587 => STARTTLS
        /// - otherwise => StartTlsWhenAvailable
        /// </summary>
        private async Task SendViaSmtpAsync(MimeMessage mime)
        {
            if (string.IsNullOrWhiteSpace(_appSettings.SmtpHost))
                throw new AppException("SMTP host is not configured (SmtpHost).");

            if (_appSettings.SmtpPort <= 0)
                throw new AppException("SMTP port is not configured (SmtpPort).");

            if (string.IsNullOrWhiteSpace(_appSettings.SmtpUser))
                throw new AppException("SMTP username is not configured (SmtpUser).");

            if (string.IsNullOrWhiteSpace(_appSettings.SmtpPass))
                throw new AppException("SMTP password is not configured (SmtpPass).");

            // Container-friendly location
            var protocolLogPath = "/tmp/smtp-protocol.log";

            using var client = new SmtpClient(new MailKit.ProtocolLogger(protocolLogPath));

            try
            {
                if (_env.IsDevelopment())
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                var host = _appSettings.SmtpHost;
                var port = _appSettings.SmtpPort;

                // Normalize secrets (CI/env vars often inject newlines/quotes)
                var user = (_appSettings.SmtpUser ?? "").Trim().TrimEnd('\r', '\n').Trim('"');
                var pass = (_appSettings.SmtpPass ?? "").Trim().TrimEnd('\r', '\n').Trim('"');

                SecureSocketOptions socketOptions =
                    port == 465 ? SecureSocketOptions.SslOnConnect :
                    port == 587 ? SecureSocketOptions.StartTls :
                    SecureSocketOptions.StartTlsWhenAvailable;

                _logger.LogInformation(
                    "SMTP config: Host={Host}, Port={Port}, User='{User}', PassLength={Len}, SocketOptions={SocketOptions}",
                    host, port, user, pass?.Length ?? 0, socketOptions
                );

                await client.ConnectAsync(host, port, socketOptions);

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

                throw new AppException($"SMTP send failed: {ex.GetType().Name} - {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates a readable plain-text fallback from HTML (good enough for deliverability).
        /// </summary>
        private static string CreateTextBodyFromHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            var text = html;

            text = Regex.Replace(text, @"<\s*br\s*/?\s*>", "\n", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"</\s*p\s*>", "\n\n", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"</\s*div\s*>", "\n", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"</\s*h[1-6]\s*>", "\n\n", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"<\s*li\s*>", "• ", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"</\s*li\s*>", "\n", RegexOptions.IgnoreCase);

            text = Regex.Replace(text, "<.*?>", string.Empty, RegexOptions.Singleline);
            text = WebUtility.HtmlDecode(text);

            text = Regex.Replace(text, @"\n{3,}", "\n\n");
            text = Regex.Replace(text, @"[ \t]{2,}", " ");

            return text.Trim();
        }

        /// <summary>
        /// Resolves nested placeholders inside values (if you use {Token} inside a value).
        /// Runs a few passes to avoid infinite loops.
        /// </summary>
        private static Dictionary<string, string> ResolveNestedPlaceholders(IDictionary<string, string> values)
        {
            var resolved = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (values == null)
                return resolved;

            foreach (var kv in values)
                resolved[kv.Key] = kv.Value ?? string.Empty;

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
SUMMARY (Changes vs your earlier version)
----------------------------------------
1) Keeps From (and sets Sender) so your FROM header is always present.
2) Removes ReplyTo usage so it compiles with your current EmailMessage entity.
3) Adds multipart/alternative (TextBody + HtmlBody) for deliverability.
4) Adds safe standard placeholders (appName/currentYear/templateTitle/siteUrl/supportEmail).
5) Message-Id domain uses settings if available, else derives from From/SMTP user.
6) Adds XML docs + readable structure + safe SMTP secret normalization.
*/
