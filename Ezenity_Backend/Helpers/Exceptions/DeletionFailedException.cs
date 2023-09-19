using System;
using System.Globalization;

namespace Ezenity_Backend.Helpers.Exceptions
{
    /// <summary>
    /// Exception for indicating that a deletion operation has failed.
    /// </summary>
    public class DeletionFailedException : AppException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeletionFailedException"/> class.
        /// </summary>
        public DeletionFailedException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeletionFailedException"/> class with a specific message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DeletionFailedException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeletionFailedException"/> class with a formatted message.
        /// </summary>
        /// <param name="message">A composite format string that contains text intermixed with zero or more format items.</param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        public DeletionFailedException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args)) { }
    }
}
