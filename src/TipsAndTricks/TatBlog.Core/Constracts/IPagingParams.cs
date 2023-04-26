namespace TatBlog.Core.Constracts;

public interface IPagingParams
{
    int PageSize { get; set; }

    int PageNumber { get; set; }

    string SortOrder { get; set; }

    string SortColumn { get; set; }
}
