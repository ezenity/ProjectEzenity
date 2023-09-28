using Ezenity_Backend.Models.Pages;
using System.Collections.Generic;

namespace Ezenity_Backend.Models
{
    /// <summary>
    /// Represents a standardized API response with a typed data payload.
    /// </summary>
    /// <typeparam name="T">The type of the data payload.</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Gets or sets the HTTP status code indicating the result of the operation. Conforms to standard HTTP status codes like 200 (OK), 400 (Bad Request), 404 (Not Found), 500 (Internal Server Error), etc.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets a human-readable message describing the outcome of the operation. Useful for debugging and logging purposes.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful. True for success, False for failure.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the actual data payload returned by the API operation. This property can be null if the operation was unsuccessful.
        /// </summary>
        public T Data { get; set; }

        public PaginationMetadata Pagination { get; set; }

        /// <summary>
        /// Gets or sets a list of errors that occurred during the API operation. This list is typically populated when the IsSuccess property is set to false.
        /// </summary>
        public List<string> Errors { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResponse{T}"/> class, setting the Errors list to an empty list.
        /// </summary>
        public ApiResponse()
        {
            Errors = new List<string>();
        }
    }
}
