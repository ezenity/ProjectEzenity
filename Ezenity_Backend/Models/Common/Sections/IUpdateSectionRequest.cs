namespace Ezenity_Backend.Models.Common.Sections
{
    public interface IUpdateSectionRequest
    {
        string Title { get; set; }
        string ContentType { get; set; }
        string Content { get; set; }
        string Layout { get; set; }
    }
}
