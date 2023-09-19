namespace Ezenity_Backend.Models.Sections
{
    /// <summary>
    /// Represents a request to update an existing section.
    /// </summary>
    public class UpdateSectionRequest
    {
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
