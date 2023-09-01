using Ezenity_Backend.Entities.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace Ezenity_Backend.Entities.Sections
{
    public class Section : ISection
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        public string ContentType { get; set; }

        [Required]
        public string Content { get; set; }

        public string Layout { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
