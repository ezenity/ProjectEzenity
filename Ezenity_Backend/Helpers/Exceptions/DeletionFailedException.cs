using System;
using System.Globalization;

namespace Ezenity_Backend.Helpers.Exceptions
{
    /// <summary>
    /// Exception for indicating that a deletion operation has failed.
    /// </summary>
    public class DeletionFailedException : AppException
    {
        public DeletionFailedException() : base() { }
        public DeletionFailedException(string message) : base(message) { }
        public DeletionFailedException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args)) { }
    }
}
