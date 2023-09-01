using Ezenity_Backend.Models.Common.Sections;

namespace Ezenity_Backend.Models.Sections
{
    public class SectionResponse : ISectionResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ContentType { get; set; }
        public string Content { get; set; }
        public string Layout { get; set; } 
    }
}
