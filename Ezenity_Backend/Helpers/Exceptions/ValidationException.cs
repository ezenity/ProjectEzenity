using System;
using System.Globalization;

namespace Ezenity_Backend.Helpers.Exceptions
{
    /// <summary>
    /// Exception for capturing validation errors in models and business logic.
    /// </summary>
    public class ValidationException : AppException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        public ValidationException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class with a specific message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ValidationException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class with a formatted message.
        /// </summary>
        /// <param name="message">A composite format string.</param>
        /// <param name="args">An array of objects to format.</param>
        public ValidationException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args)) { }
    }
}
