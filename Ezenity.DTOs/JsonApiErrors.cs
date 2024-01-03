namespace Ezenity.DTOs
{
    /// <summary>
    /// Represents an error object as per the JSON:API v1.0 specification for standardized API error responses.
    /// </summary>
    public class JsonApiErrors
    {
        /// <summary>
        /// Gets or Sets a unique identifier for this particular occurrence of the problem.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Gets or Sets a links object containing the about link.
        /// </summary>
        public ErrorLinks? Links { get; set; }

        /// <summary>
        /// Gets or Sets the HTTP status code applicable to this problem, expressed as a string value.
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Gets or Sets an application-specific error code, expressed as a string value.
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// Gets or Sets a short, human-readable summary of the problem.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Gets or Sets a human-readable explanation specific to this occurrence of the problem.
        /// </summary>
        public string? Detail { get; set; }

        /// <summary>
        /// Gets or Sets an object containing references to the source of the error.
        /// </summary>
        public ErrorSource? Source { get; set; }

        /// <summary>
        /// Gets or Sets a meta object containing non-standard meta-information about the error.
        /// </summary>
        public Dictionary<string, object>? Meta { get; set; }

        public class ErrorLinks
        {
            /// <summary>
            /// Gets or Sets a link that leads to further details about this particular occurrence of the problem.
            /// </summary>
            public string? About { get; set; }
        }

        public class ErrorSource
        {
            /// <summary>
            /// Gets or Sets a JSON Pointer [RFC6901] to the associated entity in the request document.
            /// </summary>
            public string? Pointer { get; set; }

            /// <summary>
            /// Gets or Sets a string indicating which query parameter caused the error.
            /// </summary>
            public string? Parameter { get; set; }
        }
    }
}
