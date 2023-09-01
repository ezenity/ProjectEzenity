using System;

namespace Ezenity_Backend.Entities.Common
{
    public interface ISection
    {
        int Id { get; set; }
        string Title { get; set; }
        string ContentType { get; set; }
        string Content { get; set; }
        string Layout { get; set; }
        DateTime? Created { get; set; }
        DateTime? Updated { get; set; }
    }
}
