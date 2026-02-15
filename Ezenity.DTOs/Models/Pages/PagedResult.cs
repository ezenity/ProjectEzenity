using System.Collections.Generic;

namespace Ezenity.Contracts.Models.Pages;

public class PagedResult<T>
{
    public IEnumerable<T> Data { get; set; }
    public PaginationMetadata Pagination { get; set; }
}
