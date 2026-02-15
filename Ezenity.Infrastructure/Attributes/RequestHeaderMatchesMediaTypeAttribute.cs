using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;

namespace Ezenity.Infrastructure.Attributes;

/// <summary>
/// Represents an attribute that is used to constrain an action method so it handles only HTTP requests
/// that contain one of the specified media types in one of the request headers.
/// </summary>
[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
public class RequestHeaderMatchesMediaTypeAttribute : Attribute, IActionConstraint
{
    /// <summary>
    /// Stores the media types to be matched against.
    /// </summary>
    private readonly MediaTypeCollection _mediaTypes = new ();

    /// <summary>
    /// The request header that should contain the media type to match.
    /// </summary>
    private readonly string _requestHeaderToMatch;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestHeaderMatchesMediaTypeAttribute"/> class.
    /// </summary>
    /// <param name="requestHeaderToMatch">The name of the request header to match against.</param>
    /// <param name="mediaType">The primary media type to match.</param>
    /// <param name="otherMediaTypes">Additional media types to match.</param>
    /// <exception cref="ArgumentNullException">Thrown when requestHeaderToMatch or mediaType is null.</exception>
    /// <exception cref="ArgumentException">Thrown when mediaType or otherMediaTypes are not valid media types.</exception>
    public RequestHeaderMatchesMediaTypeAttribute(string requestHeaderToMatch, string mediaType, params string[] otherMediaTypes)
    {
        _requestHeaderToMatch = requestHeaderToMatch ?? throw new ArgumentNullException(nameof(requestHeaderToMatch));

        // Check if the inputted mediatypes are valid media types
        // and add them to the _mediaType collection

        if (MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue? parsedMediaType))
            _mediaTypes.Add(parsedMediaType);
        else
            throw new ArgumentException("Argument can not be null or empty.", nameof(mediaType));

        foreach(var otherMediaType in otherMediaTypes)
        {
            if (MediaTypeHeaderValue.TryParse(otherMediaType, out MediaTypeHeaderValue? parsedOtherMediaType))
                _mediaTypes.Add(parsedOtherMediaType);
            else
                throw new ArgumentException("Arugment can not be null or empty.", nameof(otherMediaType));
        }
    }

    /// <summary>
    /// Gets the order value for determining the order of execution of action constraints.
    /// </summary>
    public int Order { get; }

    /// <summary>
    /// Determines whether this instance accepts the specified action constraint context.
    /// </summary>
    /// <param name="context">The action constraint context.</param>
    /// <returns>
    ///   <c>true</c> if this instance accepts the specified action constraint context; otherwise, <c>false</c>.
    /// </returns>
    public bool Accept(ActionConstraintContext context)
    {
        var requestHeaders = context.RouteContext.HttpContext.Request.Headers;

        if(!requestHeaders.ContainsKey(_requestHeaderToMatch))
            return false;

        var parsedRequestMediaType = new MediaType(requestHeaders[_requestHeaderToMatch]);

        // If one of the media types matches, return true
        foreach(var mediaType in _mediaTypes)
        {
            var parsedMediaType = new MediaType(mediaType);

            if(parsedRequestMediaType.Equals(parsedMediaType))
                return true;
        }

        return false;
    }
}
