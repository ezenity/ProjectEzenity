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
                // var emailTemplate = EmailHelpers.GetEmailTemplateByName(message.TemplateName, _context) ?? throw new AppException($"Email template ,'{message.TemplateName}', not found.");
                //var emailTemplate = await _context.EmailTemplates.FirstOrDefaultAsync(t => t.TemplateName == message.TemplateName) ?? throw new AppException($"Email template ,'{message.TemplateName}', not found.");
                var emailTemplate = await _emailTemplateService.GetByNameAsync(message.TemplateName);

                //Console.WriteLine($"Dynamic Values: {string.Join(", ", message.DynamicValues.Select(kv => $"{kv.Key}: {kv.Value}"))}");


                Console.WriteLine($"Before assignment, message.DynamicValues: {JsonConvert.SerializeObject(message.DynamicValues)}");
                Console.WriteLine($"Before assignment, emailTemplate.PlaceholderValues: {JsonConvert.SerializeObject(emailTemplate.PlaceholderValues)}");


                // Set the placeholders for dynamic content
                emailTemplate.PlaceholderValues = message.DynamicValues;
                //                message.DynamicValues = emailTemplate.PlaceholderValues;

                // This should populate the PlaceholderValues based on the PlaceholderValuesJson from the database.
                //emailTemplate.PlaceholderValuesJson = emailTemplate.PlaceholderValuesJson;
                Console.WriteLine($"After assignment, emailTemplate.PlaceholderValues: {JsonConvert.SerializeObject(emailTemplate.PlaceholderValues)}");

                /*if (message.DynamicValues != null)
                {
                    emailTemplate.PlaceholderValues = message.DynamicValues;
                    Console.WriteLine($"After assignment, emailTemplate.PlaceholderValues: {JsonConvert.SerializeObject(emailTemplate.PlaceholderValues)}");
                }
                else
                {
                    Console.WriteLine("Warning: message.DynamicValues is null.");
                }*/

                // Generalized placeholder replacement
                foreach (var key in emailTemplate.PlaceholderValues.Keys.ToList())
                {
                    var value = emailTemplate.PlaceholderValues[key];
                    foreach (var inneyKey in emailTemplate.PlaceholderValues.Keys)
                    {
                        var placeholder = $"{{{inneyKey}}}";
                        if (value.Contains(placeholder))
                        {
                            value = value.Replace(placeholder, emailTemplate.PlaceholderValues[inneyKey]);
                        }
                    }
                    emailTemplate.PlaceholderValues[key] = value;
                }

                //message.DynamicValues = (Dictionary<string, string>) emailTemplate.PlaceholderValues;

                // Replace placeholders in the template with dynamic values
                //string body = emailTemplate.TemplateBody;
                // Debug log
                Console.WriteLine("Placeholder Values: " + JsonConvert.SerializeObject(emailTemplate.PlaceholderValues));

                string body;
                if (!string.IsNullOrEmpty(emailTemplate.ContentViewPath) )
                {
                    // Use Razor view for rendering
                    //body = await _razorRenderer.RenderViewtoStringAsync(emailTemplate.ContentViewPath, emailTemplate.PlaceholderValues);
                    body = await _emailTemplateService.RenderEmailTemplateAsync(message.TemplateName, emailTemplate.PlaceholderValues);
                }
                else
                {
                    // Fallback to the existing ApplyDynamicContent method if ContentViewPath is not set
                    //body = emailTemplate.ApplyDynamicContent();

                    // TODO: Create a non-dynamic method to implement
                    body = "";
                }

                Console.WriteLine($"After applying dynamic content, body: {body}");

                //message.DynamicValues = emailTemplate.PlaceholderValues; // Testing location - not standard

                message.Subject = emailTemplate.Subject;

                Console.WriteLine("After setting message.Subject = emailTemplate.Subject: {0}", message.Subject);



                // This is replaced with the above line
                /*foreach (var dynamicValue in message.DynamicValues)
                    body = body.Replace($"{{{dynamicValue.Key}}}", dynamicValue.Value);*/

                //UserCredential oAuthCreds;
                //string[] Scope = { GmailService.Scope.GmailSend };
                //static string[] Scope = { GmailService.Scope.GmailModify };

                /*using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
                {
                    *//* The file token.json stores the user's access and refresh tokens, and is created
                        automatically when the authorization flow completes for the first time. *//*
                    string credPath = "token.json";

                    // Load OAuth2 creds from file
                    oAuthCreds = GoogleWebAuthorizationBroker.AuthorizeAsync(
                            GoogleClientSecrets.FromStream(stream).Secrets,
                            Scope,
                            "user",
                            CancellationToken.None,
                            new FileDataStore(credPath, true)).Result;
                    Console.WriteLine("Credential file save to: " + credPath);
                }*/

                // Send the email using SMTPClient
                using (var smtpClient = new SmtpClient())
                {
                    try
                    {
                        smtpClient.ServerCertificateValidationCallback = (s, cert, chain, sslPolicyErrors) => true;
                        Console.WriteLine("After smtpClient.ServerCertificateValidationCallback");
                        try
                        {
                            // await smtpClient.ConnectAsync(_appSettings.SmtpHost, _appSettings.SmtpPort, SecureSocketOptions.SslOnConnect);
                            //await smtpClient.ConnectAsync(_appSettings.SmtpHost, _appSettings.SmtpPort, SecureSocketOptions.StartTlsWhenAvailable);
                            //await smtpClient.ConnectAsync(_appSettings.SmtpHost, _appSettings.SmtpPort);
                            await smtpClient.ConnectAsync(
                                _appSettings.SmtpHost,
                                _appSettings.SmtpPort,
                                SecureSocketOptions.StartTls
                            );

                            Console.WriteLine("After smtpClient.ConnectAsync()");
                            Console.WriteLine("SMTP Host: {0}", _appSettings.SmtpHost);
                            Console.WriteLine("SMTP Host: {0}", _appSettings.SmtpPort);

                        } catch (SmtpCommandException ex)
                        {
                            Console.WriteLine("Error trying to connect: {0}", ex.Message);
                            Console.WriteLine("\tStatusCode: {0}", ex.StatusCode);
                            return;
                        }catch(SmtpProtocolException ex)
                        {
                            Console.WriteLine("Protocal error while trying to connect: {0}", ex.Message);
                            return;
                        }

                        Console.WriteLine("Before Try/Catch");

                        //if (_appSettings.SmtpEnableSsl)
                        //{
                        try
                        {
                            /// https://support.google.com/mail/?p=InvalidSecondFactor s22-20020a814516000000b00609f87d6d1esm974290ywa.48 - gsmtp
                            Console.WriteLine("Before smtpClient.AuthenticateAsync()");
                            //Console.WriteLine("SMTP User: {0} \n SMTP PW: {1}", _appSettings.SmtpUser, _appSettings.SmtpPass);
                            await smtpClient.AuthenticateAsync(_appSettings.SmtpUser, _appSettings.SmtpPass);
                            Console.WriteLine("After smtpClient.AuthenticateAsync()");
                            //Console.WriteLine("SMTP User: {0} \n SMTP PW: {1}", _appSettings.SmtpUser, _appSettings.SmtpPass);

                        } catch (Core.Helpers.Exceptions.AuthenticationException ex)
                        {
                            Console.WriteLine("Invalid username or password", ex);
                            return;
                        } catch (SmtpCommandException ex)
                        {
                            Console.WriteLine("Error trying to authenticate : {0}", ex.Message);
                            Console.WriteLine("\tStatusCode: {0}", ex.StatusCode);
                            return;
                        } catch (SmtpProtocolException ex)
                        {
                            Console.WriteLine("Protocal error while trying to authenticate: {0}", ex.Message);
                            return;
                        }
                        //}

                        // Create the email message
                        var emailMessage = new MimeMessage();
                        Console.WriteLine("After 'emailMessage' {0}", emailMessage);

                        // Set custom Message-Id
                        var messageId = $"{Guid.NewGuid()}@{_appSettings.EmailMessageIdDomain}";
                        emailMessage.MessageId = messageId;

                        if (_env.IsDevelopment())
                        {
                            emailMessage.From.Add(new MailboxAddress("Ezenity API Test", "noreply@ezenity.com"));
                        }
                        else
                        {
                            //emailMessage.From.Add(new MailboxAddress(message.From, message.From));
                            emailMessage.From.Add(
                                MailboxAddress.Parse(_appSettings.SmtpFrom)
                            );
                            Console.WriteLine("After From.Add() {0}", emailMessage.From.ToString() );
                            Console.WriteLine("PROD");
                        }
                        emailMessage.To.Add(new MailboxAddress(message.To, message.To));

                        //emailMessage.ReplyTo.Add(MailboxAddress.Parse(message.From));

                        Console.WriteLine("After To.Add() {0}", emailMessage.To.ToString() );
                        emailMessage.Subject = message.Subject;
                        Console.WriteLine("After Subject {0}", emailMessage.Subject);
                        emailMessage.Body = new TextPart(TextFormat.Html)
                        {
                            Text = body
                        };

                        Console.WriteLine("After Body {0}", emailMessage.Body.ToString() );

                        Console.WriteLine("After creating email message: {0}", emailMessage);

                        // Create GMail API Service
                        /*var service = new GmailService(new BaseClientService.Initializer()
                        {
                            HttpClientInitializer = oAuthCreds,
                            ApplicationName = "Project Ezenity",
                        });*/

                        // Send the email
                        //await service.Users.Messages.Send(emailMessage);
                        try
                        {
                            await smtpClient.SendAsync(emailMessage);
                            Console.WriteLine("After smtpClient.sendAsync(emailMessage)");
                        } catch (SmtpCommandException ex)
                        {
                            Console.WriteLine("Error sending message: {0}", ex.Message);
                            Console.WriteLine("\tStatusCode: {0}", ex.StatusCode);

                            switch (ex.ErrorCode)
                            {
                                case SmtpErrorCode.RecipientNotAccepted:
                                    Console.WriteLine("\tRecipient not accepted: {0}", ex.Mailbox);
                                    break;
                                case SmtpErrorCode.SenderNotAccepted:
                                    Console.WriteLine("\tSender not accepted: {0}", ex.Mailbox);
                                    break;
                                case SmtpErrorCode.MessageNotAccepted:
                                    Console.WriteLine("\tMessage not accepted.");
                                    break;
                            }
                        } catch (SmtpProtocolException ex)
                        {
                            Console.WriteLine("Protocol error while sending message: {0}", ex.Message);
                        }
                        
                        await smtpClient.DisconnectAsync(true);
                    }
                    catch (Exception smtpException)
                    {
                        // Handle specific SMTP sexceptions here
                        throw new AppException("Error sending email: {0} \n\n {1}", smtpException, smtpException.Message);
                    }
                }
            }
            catch (AppException appEx)
            {
                // Log or handle known application exceptions
                throw new AppException("An unexpected error occurred while sending the email.", appEx);  // Re-throws the caught AppException
                // throw new AppException("An unexpected error occurred while sending the email: {0} \n {1}", appEx, appEx.Message);  // Re-throws the caught AppException
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions
                // throw new AppException("An unexpected error occurred while sending the email.", ex);
                throw new AppException("An unexpected error occurred while sending the email: {0} \n\n {1}", ex, ex.Message);
            }
        }
    }
}
