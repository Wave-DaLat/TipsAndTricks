using Microsoft.AspNetCore.Mvc;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Components;

public class CategoriesWidget : ViewComponent
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoriesWidget(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        // Lấy danh sách chủ đề
        var categories = await _categoryRepository.GetCategoriesAsync();

        return View(categories);
    }
}
