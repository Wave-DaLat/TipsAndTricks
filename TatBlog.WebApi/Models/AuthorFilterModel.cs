namespace TatBlog.WebApi.Models;

public class AuthorFilterModel : PagingModel
{
    public string FullName { get; set;}

    public string Email { get; set;}
}
