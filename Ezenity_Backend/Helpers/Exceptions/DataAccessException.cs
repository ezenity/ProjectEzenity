using System;
using System.Globalization;

namespace Ezenity_Backend.Helpers.Exceptions
{
    /// <summary>
    /// Exception for capturing lower-level, unrecoverable errors when interacting with data storage.
    /// </summary>
    public class DataAccessException : AppException
    {
        public DataAccessException() : base() { }
        public DataAccessException(string message) : base(message) { }
        public DataAccessException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args)) { }
    }
}
