using Ezenity.Core.Entities.Emails;
using Ezenity.Core.Helpers.Exceptions;
using Ezenity.Core.Interfaces;
using Ezenity.Core.Services.Common;
using Ezenity.Infrastructure.Helpers;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Ezenity.Infrastructure.Services.Emails
{
    /// <summary>
    /// Provides services for sending emails.
    /// </summary>
    public class EmailService : IEmailService
    {
        /// <summary>
        /// Holds the application settings.
        /// </summary>
        private readonly IAppSettings _appSettings;

        /// <summary>
        /// The data context used for database operations.
        /// </summary>
        private readonly IDataContext _context;

        /// <summary>
        /// Provides details about the web hosting environment an application is running in.
        /// </summary>
        private readonly IWebHostEnvironment _env;

        /// <summary>
        /// The data used for Email Templates.
        /// </summary>
        private readonly IEmailTemplateService _emailTemplateService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailService"/> class.
        /// </summary>
        /// <param name="appSettings">Application settings.</param>
        /// <param name="context">Data context for database operations.</param>
        /// <param name="env">Web hosting environment details.</param>
        /// <param name="emailTemplateService">Email Template data.</param>
        public EmailService(IAppSettings appSettings, IDataContext context, IWebHostEnvironment env, IEmailTemplateService emailTemplateService)
        {
            _appSettings = appSettings ?? throw new ArgumentException(nameof(appSettings));
            _context = context ?? throw new ArgumentException(nameof(context));
            _env = env ?? throw new ArgumentException(nameof(env));
            _emailTemplateService = emailTemplateService ?? throw new ArgumentException(nameof(emailTemplateService));
        }

        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="message">The email message to send.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task SendEmailAsync(EmailMessage message)
        {
            try
            {
                // Get the email template from the database based on the provided template name
                var emailTemplate = await _emailTemplateService.GetByNameAsync(message.TemplateName);

                // Merge dynamic values fro mthe message with the template's placeholders
                var placeholderValues = MergePlaceholderValues(emailTemplate.PlaceholderValues, message.DynamicValues);

                // Render the view to a string with merged values
                string body = await _emailTemplateService.RenderEmailTemplateAsync(message.TemplateName, placeholderValues);

                // Create the email message
                var emailMessage = CreateEmailMessage(message, emailTemplate.Subject, body);

                // Send the email
                await SendEmailAsync(emailMessage);
            }
            catch (ResourceNotFoundException ex)
            {
                // Handle known exceptions
                throw new AppException("An unexpected error occurred while sending the email.", ex);
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions
                throw new AppException("An unexpected error occurred while sending the email.", ex);
            }
        }

        private Dictionary<string, string> MergePlaceholderValues(Dictionary<string, string> templateValues, Dictionary<string, string> dynamicValues)
        {
            var mergedValues = new Dictionary<string, string>(templateValues);

            foreach (var key in dynamicValues.Keys)
            {
                mergedValues[key] = dynamicValues[key];
            }

            // Perform nested placeholder replacement
            foreach (var key in mergedValues.Keys.ToList())
            {
                var value = mergedValues[key];
                foreach (var innerKey in mergedValues.Keys)
                {
                    var placeholder = $"{{{innerKey}}}";
                    if (value.Contains(placeholder))
                    {
                        value = value.Replace(placeholder, mergedValues[innerKey]);
                    }
                }
                mergedValues[key] = value;
            }
            return mergedValues;
        }

        private MimeMessage CreateEmailMessage(EmailMessage message, string subject, string body)
        {
            var emailMessage = new MimeMessage
            {
                MessageId = $"{Guid.NewGuid()}@{_appSettings.EmailMessageIdDomain}",
                Subject = subject,
                Body = new TextPart(TextFormat.Html) { Text = body }
            };

            emailMessage.From.Add(new MailboxAddress(_env.IsDevelopment() ? "Ezenity API Test" : message.From, _env.IsDevelopment() ? "noreply@ezenity.com" : message.From));
            emailMessage.To.Add(new MailboxAddress(message.To, message.To));

            return emailMessage;
        }

        private async Task SendEmailAsync(MimeMessage emailMessage)
        { 
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.ServerCertificateValidationCallback = (s, cert, chain, sslPolicyErrors) => true;

                try
                {
                    await smtpClient.ConnectAsync(_appSettings.SmtpHost, _appSettings.SmtpPort);
                    await smtpClient.AuthenticateAsync(_appSettings.SmtpUser, _appSettings.SmtpPass);
                    await smtpClient.SendAsync(emailMessage);
                    await smtpClient.DisconnectAsync(true);
                }
                catch (SmtpCommandException ex)
                {
                    // Handle specific SMTP command exceptions
                    HandleSmtpCommandException(ex);
                }
                catch (SmtpProtocolException ex)
                {
                    // Handle specific SMTP protocol exceptions
                    Console.WriteLine($"Protocol error while sending message: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Handle any other exceptions
                    throw new AppException("Error sending email.", ex);
                }
            }
        }

        private void HandleSmtpCommandException(SmtpCommandException ex)
        {
            switch (ex.ErrorCode)
            {
                case SmtpErrorCode.RecipientNotAccepted:
                    Console.WriteLine($"Recipient not accepted: {ex.Mailbox}");
                    break;
                case SmtpErrorCode.SenderNotAccepted:
                    Console.WriteLine($"Sender not accepted: {ex.Mailbox}");
                    break;
                case SmtpErrorCode.MessageNotAccepted:
                    Console.WriteLine("Message not accepted.");
                    break;
                default:
                    Console.WriteLine($"Error sending message: {ex.Message}");
                    break;
            }
            throw new AppException($"SMTP command error: {ex.Message}", ex);
        }

    }
}
