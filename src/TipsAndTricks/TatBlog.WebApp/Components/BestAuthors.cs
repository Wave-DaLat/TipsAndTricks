using Microsoft.AspNetCore.Mvc;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Components;

public class BestAuthors : ViewComponent
{
    private readonly IAuthorRepository _authorRepository;

    public BestAuthors(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var authors = await _authorRepository.Find_N_PostsByAuthorAsync(4);

        return View(authors);
    }
}
