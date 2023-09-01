using Ezenity_Backend.Entities;
using Ezenity_Backend.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ezenity_Backend.Helpers
{
    public static class EmailHelpers
    {
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

        public static string ExtractEmailAddress(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var regex = new Regex(@"[\w-\.]+@([\w-]+\.)+[\w-]{2,4}");
            var match = regex.Match(input);
            return match.Success ? match.Value : string.Empty;
        }

        public static string CommaSeparatedEmails(IEnumerable<string> emailList)
        {
            return string.Join(",", emailList.Where(IsValidEmail));
        }

        public static IEmailTemplate GetEmailTemplateByName(string templateName, DataContext context, string version)
        {
            return context.EmailTemplates.FirstOrDefault(t => t.TemplateName == templateName);
        }
    }
}
