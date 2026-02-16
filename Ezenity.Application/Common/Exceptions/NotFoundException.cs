using System;
using System.Globalization;

namespace Ezenity.Application.Common.Exceptions;

/// <summary>
/// General exception for not finding a resource, whether it's a data record or file.
/// </summary>
public class NotFoundException : AppException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class.
    /// </summary>
    public NotFoundException() : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class with a specific message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NotFoundException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class with a formatted message.
    /// </summary>
    /// <param name="message">A composite format string that contains text intermixed with zero or more format items.</param>
    /// <param name="args">An object array containing zero or more objects to format.</param>
    public NotFoundException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args)) { }
}
