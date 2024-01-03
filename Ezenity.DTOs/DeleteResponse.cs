using Ezenity.DTOs.Models.Accounts;

namespace Ezenity.DTOs.Models
{
    /// <summary>
    /// Represents a response containing details about the deletion operation.
    /// </summary>
    public class DeleteResponse
    {
        /// <summary>
        /// Gets or sets a human-readable message describing the deletion outcome.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code indicating the deletion result.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the UTC time when the resource was deleted.
        /// </summary>
        public DateTime DeletedAt { get; set; }

        /// <summary>
        /// Gets or sets the account responsible for the deletion.
        /// </summary>
        public AccountResponse DeletedBy { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the deleted resource.
        /// </summary>
        public string ResourceId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the success or failure of the deletion.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets a list of errors, if any, that occurred during the deletion.
        /// </summary>
        public List<JsonApiErrors> Errors { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteResponse"/> class, setting the Errors list to an empty list.
        /// </summary>
        public DeleteResponse()
        {
            Errors = new List<JsonApiErrors>();
        }
    }
}
