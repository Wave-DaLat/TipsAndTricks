using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using TatBlog.Core.DTO;
using TatBlog.Services.Blogs;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Areas.Admin.Controllers;

public class SubscribersController : Controller
{
    private readonly ILogger<PostsController> _logger;
    private readonly ISubscriberRepository _subscriberRepository;
    private readonly IMapper _mapper;

    public SubscribersController(ISubscriberRepository subscriberRepository, IMapper mapper, ILogger<PostsController> logger)
    {
        _subscriberRepository = subscriberRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IActionResult> Index(SubscriberFilterModel model, [FromQuery(Name = "p")] int pageNumber = 1, [FromQuery(Name = "ps")] int pageSize = 10)
    {
        var subscriberQuery = _mapper.Map<SubscriberQuery>(model);

        ViewData["SubscribersList"] = await _subscriberRepository.GetSubscriberByQueryAsync(subscriberQuery, pageNumber, pageSize);
        ViewData["PagerQuery"] = new PagerQuery
        {
            Area = "Admin",
            Controller = "Subscribers",
            Action = "Index",
        };

        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> DeleteSubscriber(string id)
    {
        await _subscriberRepository.DeleteSubscriberAsync(Convert.ToInt32(id));

        return RedirectToAction(nameof(Index));
    }
}
