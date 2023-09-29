using System;
using System.Globalization;

namespace Ezenity.Core.Helpers.Exceptions
{
    /// <summary>
    /// Exception for capturing issues related to concurrent access to shared resources.
    /// </summary>
    public class ConcurrencyException : AppException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrencyException"/> class.
        /// </summary>
        public ConcurrencyException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrencyException"/> class with a specific message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ConcurrencyException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrencyException"/> class with a formatted message.
        /// </summary>
        /// <param name="message">A composite format string that contains text intermixed with zero or more format items, which correspond to objects in the args array.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public ConcurrencyException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args)) { }
    }
}
