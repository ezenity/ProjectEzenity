using System;
using System.Globalization;

namespace Ezenity.Core.Helpers.Exceptions
{
    /// <summary>
    /// Exception for capturing failures due to external services or components being unavailable or failing.
    /// </summary>
    public class DependencyFailureException : AppException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyFailureException"/> class.
        /// </summary>
        public DependencyFailureException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyFailureException"/> class with a specific message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DependencyFailureException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyFailureException"/> class with a formatted message.
        /// </summary>
        /// <param name="message">A composite format string that contains text intermixed with zero or more format items.</param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        public DependencyFailureException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args)) { }
    }
}
