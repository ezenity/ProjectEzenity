using System;
using System.Globalization;

namespace Ezenity.Core.Helpers.Exceptions
{
    /// <summary>
    /// Exception specifically for capturing issues related to invalid or expired verification tokens.
    /// </summary>
    public class InvalidVerificationTokenException : AppException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidVerificationTokenException"/> class.
        /// </summary>
        public InvalidVerificationTokenException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidVerificationTokenException"/> class with a specific message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidVerificationTokenException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidVerificationTokenException"/> class with a formatted message.
        /// </summary>
        /// <param name="message">A composite format string that contains text intermixed with zero or more format items.</param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        public InvalidVerificationTokenException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
