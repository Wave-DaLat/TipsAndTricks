namespace TatBlog.Core.DTO;

public class PostQuery
{
    public string Keyword { get; set; }

    public bool PublishedOnly { get; set; }

    public bool NotPublished { get; set; }

    public int? Year { get; set; }

    public int? Month { get; set; }

    public int? Day { get; set; }

    public int? AuthorId { get; set; }

    public int? CategoryId { get; set; }

    public string Tags { get; set; }

    public string PostSlug { get; set; }

    public string AuthorSlug { get; set; }

    public string CategorySlug { get; set; }

    public string TagSlug { get; set; }

    public IList<string> SelectedTag { get; set; }

    public void GetTagListAsync()
    {
        SelectedTag = (Tags ?? "").Split(new[] { ",", ";", ".", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
    }
}