using Microsoft.AspNetCore.Mvc;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Areas.Admin.Controllers;

public class DashboardController : Controller
{
    private readonly IDashboardRepository _dashboardRepository;

    public DashboardController(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["TotalOfPosts"] = await _dashboardRepository.GetTotalOfPostsAsync();

        ViewData["TotalOfUnpublishedPosts"] = await _dashboardRepository.GetTotalOfUnpublishedPostsAsync();

        ViewData["TotalOfCategories"] = await _dashboardRepository.GetTotalOfCategoriesAsync();

        ViewData["TotalOfAuthors"] = await _dashboardRepository.GetTotalOfAuthorsAsync();

        ViewData["TotalOfWaitingComment"] = await _dashboardRepository.GetTotalOfWaitingCommentAsync();

        ViewData["TotalOfSubscriber"] = await _dashboardRepository.GetTotalOfSubscriberAsync();

        ViewData["TotalOfNewestSubscriberInDay"] = await _dashboardRepository.GetTotalOfNewestSubscriberInDayAsync();

        return View();
    }
}
