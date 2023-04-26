using FluentValidation;
using FluentValidation.AspNetCore;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Areas.Admin.Controllers;

public class CategoriesController : Controller
{
    private readonly ILogger<PostsController> _logger;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CategoryEditModel> _categoryValidator;

    public CategoriesController(ILogger<PostsController> logger, ICategoryRepository categoryRepository, IMapper mapper, IValidator<CategoryEditModel> categoryValidator)
    {
        _logger = logger;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _categoryValidator = categoryValidator;
    }

    public async Task<IActionResult> Index(CategoryFilterModel model,
                                          [FromQuery(Name = "p")] int pageNumber = 1,
                                          [FromQuery(Name = "ps")] int pageSize = 10)
    {
        var categoryQuery = _mapper.Map<CategoryQuery>(model);

        ViewData["CategoriesList"] = await _categoryRepository.GetCategoryByQueryAsync(categoryQuery, pageNumber, pageSize);

        ViewData["PagerQuery"] = new PagerQuery
        {
            Area = "Admin",
            Controller = "Categories",
            Action = "Index",
        };

        return View(model);
    }

    [HttpGet]
    public async Task<ActionResult> Edit(int id = 0)
    {
        var category = id > 0 ? await _categoryRepository.GetCategoryByIdAsync(id) : null;

        var model = category == null ? new CategoryEditModel() : _mapper.Map<CategoryEditModel>(category);

        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> Edit(CategoryEditModel model)
    {
        var validator = HttpContext.RequestServices.GetService(typeof(IValidator<CategoryEditModel>));
        var validationResult = await _categoryValidator.ValidateAsync(model);

        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
        }

        if (!ModelState.IsValid)
            return View(model);

        var category = model.Id > 0 ? await _categoryRepository.GetCategoryByIdAsync(model.Id) : null;

        if (category == null)
        {
            category = _mapper.Map<Category>(model);

            category.Id = 0;
        }
        else
        {
            _mapper.Map(model, category);
        }

        await _categoryRepository.AddOrUpdateCategoryAsync(category);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<ActionResult> VerifyCategorySlug(int id, string urlSlug)
    {
        var slugExisted = await _categoryRepository.CheckCategorySlugExisted(id, urlSlug);

        return slugExisted ? Json($"Slug '{urlSlug}' đã được sử dụng") : Json(true);
    }

    [HttpPost]
    public async Task<ActionResult> ShowedChanged(string categoryId)
    {
        await _categoryRepository.ChangeCategoryStatusAsync(Convert.ToInt32(categoryId));

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<ActionResult> DeleteCategory(string id)
    {
        await _categoryRepository.DeleteCategoryByIdAsync(Convert.ToInt32(id));

        return RedirectToAction(nameof(Index));
    }
}
