using System;
using System.Globalization;

namespace Ezenity_Backend.Helpers.Exceptions
{
    /// <summary>
    /// Exception for capturing authorization failures, such as insufficient permissions.
    /// </summary>
    public class AuthorizationException : AppException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationException"/> class.
        /// </summary>
        public AuthorizationException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationException"/> class with a specific message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public AuthorizationException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationException"/> class with a formatted message.
        /// </summary>
        /// <param name="message">A composite format string that contains text intermixed with zero or more format items, which correspond to objects in the args array.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public AuthorizationException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args)) { }
    }
}
