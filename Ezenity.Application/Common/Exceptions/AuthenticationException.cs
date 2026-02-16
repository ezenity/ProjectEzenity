using System.Globalization;

namespace Ezenity.Application.Common.Exceptions;

/// <summary>
/// Exception for capturing authentication-related issues, such as invalid credentials.
/// </summary>
public class AuthenticationException : AppException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationException"/> class.
    /// </summary>
    public AuthenticationException() : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationException"/> class with a specific message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public AuthenticationException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationException"/> class with a formatted message.
    /// </summary>
    /// <param name="message">A composite format string that contains text intermixed with zero or more format items, which correspond to objects in the args array.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public AuthenticationException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args)) { }
}
