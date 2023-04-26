namespace TatBlog.WebApi.Models;

public class CommentDto
{
    public int Id { get; set; }

    public string UserName { get; set; }

    public string Content { get; set; }

    public DateTime PostDate { get; set; }

    public bool Censored { get; set; }

    public int PostID { get; set; }

    public PostDto Post { get; set; }
}

