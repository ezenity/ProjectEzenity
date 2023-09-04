using System;
using System.Globalization;

namespace Ezenity_Backend.Helpers.Exceptions
{
    /// <summary>
    /// Exception for capturing authorization failures, such as insufficient permissions.
    /// </summary>
    public class AuthorizationException : AppException
    {
        public AuthorizationException() : base() { }
        public AuthorizationException(string message) : base(message) { }
        public AuthorizationException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args)) { }
    }
}
