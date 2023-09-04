using System;
using System.Globalization;

namespace Ezenity_Backend.Helpers.Exceptions
{
    /// <summary>
    /// Exception for capturing situations where a resource already exists when trying to create a new one.
    /// </summary>
    public class ResourceAlreadyExistsException : AppException
    {
        public ResourceAlreadyExistsException() : base() { }
        public ResourceAlreadyExistsException(string message) : base(message) { }
        public ResourceAlreadyExistsException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args)) { }
    }
}
