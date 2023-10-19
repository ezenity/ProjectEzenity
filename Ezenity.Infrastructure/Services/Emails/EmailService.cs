using Ezenity.Core.Entities.Emails;
using Ezenity.Core.Helpers.Exceptions;
using Ezenity.Core.Interfaces;
using Ezenity.Core.Services.Common;
using Ezenity.Infrastructure.Helpers;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
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
        /// Initializes a new instance of the <see cref="EmailService"/> class.
        /// </summary>
        /// <param name="appSettings">Application settings.</param>
        /// <param name="context">Data context for database operations.</param>
        /// <param name="env">Web hosting environment details.</param>
        public EmailService(IAppSettings appSettings, IDataContext context, IWebHostEnvironment env)
        {
            _appSettings = appSettings ?? throw new ArgumentException(nameof(appSettings));
            _context = context ?? throw new ArgumentException(nameof(context));
            _env = env ?? throw new ArgumentException(nameof(env));
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
                var emailTemplate = EmailHelpers.GetEmailTemplateByName(message.TemplateName, _context);

                // Check if the template exists
                if (emailTemplate == null)
                    throw new AppException($"Email template ,'{message.TemplateName}', not found.");

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



                //message.DynamicValues = (Dictionary<string, string>) emailTemplate.PlaceholderValues;

                // Replace placeholders in the template with dynamic values
                //string body = emailTemplate.TemplateBody;
                // Debug log
                Console.WriteLine("Placeholder Values: " + JsonConvert.SerializeObject(emailTemplate.PlaceholderValues));

                // Apply dynamic content to the template
                string body = emailTemplate.ApplyDynamicContent();

                Console.WriteLine($"After applying dynamic content, body: {body}");

                //message.DynamicValues = emailTemplate.PlaceholderValues; // Testing location - not standard

                message.Subject = emailTemplate.Subject;


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
                        try
                        {
                            await smtpClient.ConnectAsync(_appSettings.SmtpHost, _appSettings.SmtpPort, SecureSocketOptions.SslOnConnect);
                            //await smtpClient.ConnectAsync(_appSettings.SmtpHost, _appSettings.SmtpPort, SecureSocketOptions.StartTlsWhenAvailable);
                            //await smtpClient.ConnectAsync(_appSettings.SmtpHost, _appSettings.SmtpPort);

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

                        //if (_appSettings.SmtpEnableSsl)
                        //{
                        try
                        {
                            await smtpClient.AuthenticateAsync(_appSettings.SmtpUser, _appSettings.SmtpPass);
                        } catch (Core.Helpers.Exceptions.AuthenticationException ex)
                        {
                            Console.WriteLine("Invalid username or passowrd", ex);
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
                        if(_env.IsDevelopment())
                            emailMessage.From.Add(new MailboxAddress("Ezenity API Test", "anthonymmacallister@gmail.com"));
                        else
                            emailMessage.From.Add(new MailboxAddress(message.From, message.From));
                        emailMessage.To.Add(new MailboxAddress(message.To, message.To));
                        emailMessage.Subject = message.Subject;
                        emailMessage.Body = new TextPart(TextFormat.Html)
                        {
                            Text = body
                        };

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
                        throw new AppException("Error sending email", smtpException);
                    }
                }
            }
            catch (AppException appEx)
            {
                // Log or handle known application exceptions
                throw new AppException("An unexpected error occurred while sending the email.", appEx);  // Re-throws the caught AppException
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions
                throw new AppException("An unexpected error occurred while sending the email.", ex);
            }
        }
    }
}
