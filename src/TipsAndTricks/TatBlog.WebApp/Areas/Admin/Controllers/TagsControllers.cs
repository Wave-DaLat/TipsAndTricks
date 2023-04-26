using FluentValidation;
using FluentValidation.AspNetCore;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Areas.Admin.Controllers;

public class TagsController : Controller
{
    private readonly ILogger<PostsController> _logger;
    private readonly ITagRepository _tagRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<TagEditModel> _tagValidator;

    public TagsController(ITagRepository tagRepository, IMapper mapper, ILogger<PostsController> logger, IValidator<TagEditModel> tagValidator)
    {
        _tagRepository = tagRepository;
        _mapper = mapper;
        _logger = logger;
        _tagValidator = tagValidator;
    }

    public async Task<IActionResult> Index(TagFilterModel model, [FromQuery(Name = "p")] int pageNumber = 1, [FromQuery(Name = "ps")] int pageSize = 10)
    {
        // _logger.LogInformation("Tạo điều kiện truy vấn");
        var tagQuery = _mapper.Map<TagQuery>(model);

        // _logger.LogInformation("Lấy danh sách bài viết từ CSDL");

        ViewData["TagsList"] = await _tagRepository.GetTagByQueryAsync(tagQuery, pageNumber, pageSize);
        ViewData["PagerQuery"] = new PagerQuery
        {
            Area = "Admin",
            Controller = "Tags",
            Action = "Index",
        };

        return View(model);
    }

    [HttpGet]
    public async Task<ActionResult> Edit(int id = 0)
    {
        var tag = id > 0 ? await _tagRepository.GetTagByIdAsync(id) : null;

        var model = tag == null ? new TagEditModel() : _mapper.Map<TagEditModel>(tag);

        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> Edit(TagEditModel model)
    {
        var validator = HttpContext.RequestServices.GetService(typeof(IValidator<TagEditModel>));
        var validationResult = await _tagValidator.ValidateAsync(model);

        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var tag = model.Id > 0 ? await _tagRepository.GetTagByIdAsync(model.Id) : null;

        if (tag == null)
        {
            tag = _mapper.Map<Tag>(model);

            tag.Id = 0;
        }
        else
        {
            _mapper.Map(model, tag);
        }

        await _tagRepository.AddOrUpdateTagAsync(tag);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<ActionResult> VerifyTagSlug(int id, string urlSlug)
    {
        var slugExisted = await _tagRepository.CheckTagSlugExisted(id, urlSlug);

        return slugExisted ? Json($"Slug '{urlSlug}' đã được sử dụng") : Json(true);
    }

    [HttpPost]
    public async Task<ActionResult> DeleteTag(string id)
    {
        await _tagRepository.DeleteTagByIdAsync(Convert.ToInt32(id));

        return RedirectToAction(nameof(Index));
    }
}
