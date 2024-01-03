using Ezenity.DTOs.Models.Pages;
using System;
using System.Collections.Generic;

namespace Ezenity.DTOs.Models
{
    /// <summary>
    /// Represents a verbose API response that includes additional metadata for debugging and auditing.
    /// </summary>
    /// <typeparam name="T">The type of the data payload.</typeparam>
    public class VerboseApiResponse<T>
    {
        /// <summary>
        /// Gets or sets the HTTP status code indicating the result of the operation.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets a human-readable message describing the outcome of the operation.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the actual data payload returned by the API operation.
        /// </summary>
        public T Data { get; set; }

        public PaginationMetadata Pagination { get; set; }

        /// <summary>
        /// Gets or sets a list of errors that occurred during the API operation.
        /// </summary>
        public List<JsonApiErrors> Errors { get; set; }

        /// <summary>
        /// Gets or sets additional debug information.
        /// </summary>
        public string DebugInfo { get; set; }

        /// <summary>
        /// Gets or sets the UTC time when the response was generated.
        /// </summary>
        public DateTime TimeGenerated { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VerboseApiResponse{T}" /> class.
        /// </summary>
        public VerboseApiResponse()
        {
            Errors = new List<JsonApiErrors>();
            TimeGenerated = DateTime.UtcNow;
        }

        /// <summary>
        /// Sets additional debug information.
        /// </summary>
        /// <param name="info">The debug information.</param>
        public void SetDebugInfo(string info)
        {
            DebugInfo = info;
        }
    }
}
