using System.ComponentModel.DataAnnotations;

namespace Ezenity.DTOs.Models.Sections
{
    /// <summary>
    /// Represents a request to create a new section.
    /// </summary>
    public class CreateSectionRequest
    {
        /// <summary>
        /// Gets or sets the title of the section.
        /// </summary>
        [Required(ErrorMessage = "The section's title is required.")]
        [MaxLength(100)]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the type of content in the section.
        /// </summary>
        [Required]
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the content of the section.
        /// </summary>
        [Required]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the layout of the section.
        /// </summary>
        [Required]
        public string Layout { get; set; }
    }
}
