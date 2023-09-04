using System;
using System.Globalization;

namespace Ezenity_Backend.Helpers.Exceptions
{
    /// <summary>
    /// Exception for capturing issues related to concurrent access to shared resources.
    /// </summary>
    public class ConcurrencyException : AppException
    {
        public ConcurrencyException() : base() { }
        public ConcurrencyException(string message) : base(message) { }
        public ConcurrencyException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args)) { }
    }
}
