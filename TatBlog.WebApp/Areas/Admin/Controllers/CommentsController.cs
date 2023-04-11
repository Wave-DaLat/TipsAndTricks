using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using TatBlog.Core.DTO;
using TatBlog.Services.Blogs;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Areas.Admin.Controllers;

public class CommentsController : Controller
{
    private readonly ILogger<PostsController> _logger;
    private readonly ICommentRepository _commentRepository;
    private readonly IMapper _mapper;

    public CommentsController(ICommentRepository commentRepository, IMapper mapper, ILogger<PostsController> logger)
    {
        _commentRepository = commentRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IActionResult> Index(CommentFilterModel model, [FromQuery(Name = "p")] int pageNumber = 1, [FromQuery(Name = "ps")] int pageSize = 10)
    {
        var commentQuery = _mapper.Map<CommentQuery>(model);

        ViewData["CommentsList"] = await _commentRepository.GetCommentByQueryAsync(commentQuery, pageNumber, pageSize);
        ViewData["PagerQuery"] = new PagerQuery
        {
            Area = "Admin",
            Controller = "Comments",
            Action = "Index",
        };

        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> CensoredChanged(string commentId)
    {
        await _commentRepository.ChangeCommentStatusAsync(Convert.ToInt32(commentId));

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<ActionResult> DeleteComment(string id)
    {
        await _commentRepository.DeleteCommentByIdAsync(Convert.ToInt32(id));

        return RedirectToAction(nameof(Index));
    }
}
