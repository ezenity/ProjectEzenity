using System.Globalization;

namespace Ezenity.Core.Helpers.Exceptions
{
    /// <summary>
    /// Represents a custom exception for handling application-specific issues, such as validation errors.
    /// </summary>
    public class AppException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppException"/> class.
        /// </summary>
        public AppException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppException"/> class with a specific error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public AppException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppException"/> class with a formatted error message.
        /// </summary>
        /// <param name="message">A composite format string that contains text intermixed with zero or more format items, which correspond to objects in the <paramref name="args"/> array.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public AppException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args)) { }
    }
}
