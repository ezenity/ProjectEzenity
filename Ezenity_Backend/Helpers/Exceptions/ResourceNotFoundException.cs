using System;
using System.Globalization;

namespace Ezenity_Backend.Helpers.Exceptions
{
    /// <summary>
    /// Exception for indicating that a requested resource could not be found.
    /// </summary>
    public class ResourceNotFoundException : AppException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceNotFoundException"/> class.
        /// </summary>
        public ResourceNotFoundException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceNotFoundException"/> class with a specific message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ResourceNotFoundException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceNotFoundException"/> class with a formatted message.
        /// </summary>
        /// <param name="message">A composite format string.</param>
        /// <param name="args">An array of objects to format.</param>
        public ResourceNotFoundException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args)) { }
    }
}
