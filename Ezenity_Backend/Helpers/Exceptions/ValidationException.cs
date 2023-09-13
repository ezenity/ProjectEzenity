using System;
using System.Globalization;

namespace Ezenity_Backend.Helpers.Exceptions
{
    /// <summary>
    /// Exception for capturing validation errors in models and business logic.
    /// </summary>
    public class ValidationException : AppException
    {
        public ValidationException() : base() { }
        public ValidationException(string message) : base(message) { }
        public ValidationException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args)) { }
    }
}
