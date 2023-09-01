namespace Ezenity_Backend.Models.Common.Sections
{
    public interface ICreateSectionRequest
    {
        string Title { get; set; }
        string ContentType { get; set; }
        string Content { get; set; }
        string Layout { get; set; }

    }
}
