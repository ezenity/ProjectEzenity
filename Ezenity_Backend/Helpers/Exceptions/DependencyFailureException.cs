using System;
using System.Globalization;

namespace Ezenity_Backend.Helpers.Exceptions
{
    /// <summary>
    /// Exception for capturing failures due to external services or components being unavailable or failing.
    /// </summary>
    public class DependencyFailureException : AppException
    {
        public DependencyFailureException() : base() { }
        public DependencyFailureException(string message) : base(message) { }
        public DependencyFailureException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args)) { }
    }
}
