using System;
using System.Globalization;

namespace Ezenity_Backend.Helpers.Exceptions
{
    public class ResourceNotFoundException : AppException
    {
        public ResourceNotFoundException() : base() { }
        public ResourceNotFoundException(string message) : base(message) { }
        public ResourceNotFoundException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args)) { }
    }
}
