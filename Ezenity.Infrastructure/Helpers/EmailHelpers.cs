using Ezenity.Core.Entities.EmailTemplates;
using Ezenity.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ezenity.Infrastructure.Helpers
{
    /// <summary>
    /// Provides utility methods for handling and validating emails.
    /// </summary>
    public static class EmailHelpers
    {
        /// <summary>
        /// Validates the provided email string.
        /// </summary>
        /// <param name="email">The email string to validate.</param>
        /// <returns>true if the email is valid, otherwise false.</returns>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Ue Regex pattern to validate the email format
                var regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Extracts email addresses from the provided input string.
        /// </summary>
        /// <param name="input">Input string potentially containing an email.</param>
        /// <returns>The email address if found; otherwise, an empty string.</returns>
        public static string ExtractEmailAddress(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var regex = new Regex(@"[\w-\.]+@([\w-]+\.)+[\w-]{2,4}");
            var match = regex.Match(input);
            return match.Success ? match.Value : string.Empty;
        }

        /// <summary>
        /// Converts an IEnumerable of email strings to a single comma-separated string.
        /// </summary>
        /// <param name="emailList">IEnumerable of email strings.</param>
        /// <returns>A comma-separated string of emails.</returns>
        public static string CommaSeparatedEmails(IEnumerable<string> emailList)
        {
            return string.Join(",", emailList.Where(IsValidEmail));
        }
    }
}
