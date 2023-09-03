using System;
using System.Globalization;

namespace Ezenity_Backend.Helpers.Exceptions
{
    /// <summary>
    /// Exception for capturing authentication-related issues, such as invalid credentials.
    /// </summary>
    public class AuthenticationException : AppException
    {
        public AuthenticationException() : base() { }
        public AuthenticationException(string message) : base(message) { }
        public AuthenticationException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args)) { }
    }
}
