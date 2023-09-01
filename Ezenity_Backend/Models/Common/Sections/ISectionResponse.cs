namespace Ezenity_Backend.Models.Common.Sections
{
    public interface ISectionResponse
    {
        int Id { get; set; }
        string Title { get; set; }
        string ContentType { get; set; }
        string Content { get; set; }
        string Layout { get; set; } 
    }
}
