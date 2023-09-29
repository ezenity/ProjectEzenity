namespace Ezenity.DTOs.Models.Sections
{
    /// <summary>
    /// Represents a response containing section details.
    /// </summary>
    public class SectionResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier for the section.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the section.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the type of content in the section.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the content of the section.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the layout of the section.
        /// </summary>
        public string Layout { get; set; }
    }
}
