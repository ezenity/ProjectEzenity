namespace Ezenity.Core.Entities.Emails
{
    /// <summary>
    /// Represents an email message to be sent.
    /// </summary>
    public class EmailMessage
    {
        /// <summary>
        /// Gets or sets the recipient email address.
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// Gets or sets the subject of the email.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the name of the template to use for this email.
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// Gets or sets the dynamic values to be substituted in the email template.
        /// </summary>
        public Dictionary<string, string> DynamicValues { get; set; }

        /// <summary>
        /// Gets or sets the sender's email address.
        /// </summary>
        public string From { get; set; }
    }
}
