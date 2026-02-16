using System;
using System.Globalization;

namespace Ezenity.Application.Common.Exceptions;

/// <summary>
/// Exception for capturing API rate-limiting issues, useful for limiting resource usage.
/// </summary>
public class RateLimitExceededException : AppException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RateLimitExceededException"/> class.
    /// </summary>
    public RateLimitExceededException() : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RateLimitExceededException"/> class with a specific message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public RateLimitExceededException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RateLimitExceededException"/> class with a formatted message.
    /// </summary>
    /// <param name="message">A composite format string that contains text intermixed with zero or more format items.</param>
    /// <param name="args">An object array containing zero or more objects to format.</param>
    public RateLimitExceededException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args)) { }
}
