using Microsoft.AspNetCore.Mvc;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Components;

public class TagCloud : ViewComponent
{
    private readonly ITagRepository _tagRepository;

    public TagCloud(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var tags = await _tagRepository.GetTagListAsync();

        return View(tags);
    }
}
