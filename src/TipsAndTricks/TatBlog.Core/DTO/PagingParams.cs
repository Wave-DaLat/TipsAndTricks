using TatBlog.Core.Constracts;

namespace TatBlog.Core.DTO;

public class PagingParams : IPagingParams
{
    public int PageSize { get; set; }

    public int PageNumber { get; set; }

    public string SortColumn { get; set; }

    public string SortOrder { get; set; }
}
