using System;
using System.Globalization;

namespace Ezenity_Backend.Helpers.Exceptions
{
    /// <summary>
    /// Exception for capturing API rate-limiting issues, useful for limiting resource usage.
    /// </summary>
    public class RateLimitExceededException : AppException
    {
        public RateLimitExceededException() : base() { }
        public RateLimitExceededException(string message) : base(message) { }
        public RateLimitExceededException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args)) { }
    }
}
