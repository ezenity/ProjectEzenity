using System;
using System.Globalization;

namespace Ezenity_Backend.Helpers.Exceptions
{
    /// <summary>
    /// Exception for capturing specific business rule or domain constraint violations.
    /// </summary>
    public class BusinessRuleException : AppException
    {
        public BusinessRuleException() : base() { }
        public BusinessRuleException(string message) : base(message) { }
        public BusinessRuleException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args)) { }
    }
}
