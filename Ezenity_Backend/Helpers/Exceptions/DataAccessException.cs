using System;
using System.Globalization;

namespace Ezenity_Backend.Helpers.Exceptions
{
    /// <summary>
    /// Exception for capturing lower-level, unrecoverable errors when interacting with data storage.
    /// </summary>
    public class DataAccessException : AppException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataAccessException"/> class.
        /// </summary>
        public DataAccessException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAccessException"/> class with a specific message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DataAccessException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAccessException"/> class with a formatted message.
        /// </summary>
        /// <param name="message">A composite format string that contains text intermixed with zero or more format items.</param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        public DataAccessException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args)) { }
    }
}
