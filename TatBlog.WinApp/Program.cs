using TatBlog.Data.Contexts;
using TatBlog.Services.Blogs;
using TatBlog.WinApp;

var context = new BlogDbContext();

IBlogRepository blogrepo = new BlogRepository(context);

var pagingParams = new PagingParams
{
    PageNumber = 1,
    PageSize = 20,
    SortColumn = "name",
    SortOrder = "desc",
};

var tagslist = await blogrepo.GetPagedTagsAsync(pagingParams);

Console.WriteLine("{0, -5}{1, -50}{2, 10}",
    "id", " name", "count");

foreach (var item in tagslist)
{
    Console.WriteLine("{0, -5}{1, -50}{2, 10}",
        item.Id, item.Name, item.PostCount);
}