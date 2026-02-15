namespace Ezenity.Domain.Entities.Sections
{
    /// <summary>
    /// Represents a section in the system, typically a part of a page or component.
    /// </summary>
    public class Section
    {
        /// <summary>
        /// Gets or sets the unique identifier for the section.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the section.
        /// Required. Maximum length is 100 characters.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the type of content that the section holds.
        /// This could be text, HTML, markdown, etc.
        /// Required.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the actual content of the section.
        /// Required.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the layout type or template that the section uses.
        /// This is optional and could be used for custom rendering.
        /// </summary>
        public string Layout { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the section was created.
        /// </summary>
        public DateTime? Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the section was last updated.
        /// </summary>
        public DateTime? Updated { get; set; }

        /// <summary>
        /// Gets or sets the visibility control
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets who can see the section
        /// </summary>
        public string AccessLevel { get; set; }

        /// <summary>
        /// Get or set the ID for nested sections
        /// </summary>
        public int? ParentSectionId { get; set; }

        /// <summary>
        /// Get or set the SEO meta tags that could be applied to the section
        /// </summary>
        public string MetaTags { get; set; }

    }
}
