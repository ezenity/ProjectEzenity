using System;
using System.Globalization;

namespace Ezenity_Backend.Helpers.Exceptions
{
    /// <summary>
    /// Exception for capturing situations where a resource already exists when trying to create a new one.
    /// </summary>
    public class ResourceAlreadyExistsException : AppException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceAlreadyExistsException"/> class.
        /// </summary>
        public ResourceAlreadyExistsException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceAlreadyExistsException"/> class with a specific message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ResourceAlreadyExistsException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceAlreadyExistsException"/> class with a formatted message.
        /// </summary>
        /// <param name="message">A composite format string.</param>
        /// <param name="args">An array of objects to format.</param>
        public ResourceAlreadyExistsException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args)) { }
    }
}
