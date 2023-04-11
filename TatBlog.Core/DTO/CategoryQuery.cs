namespace TatBlog.Core.DTO;

public class CategoryQuery
{
    public string Keyword { get; set; }

    public string UrlSlug { get; set; }

    public bool ShowOnMenu { get; set; }
}