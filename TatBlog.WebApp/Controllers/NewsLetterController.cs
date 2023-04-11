using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Controllers;

public class NewsLetterController : Controller
{
    private readonly ISubscriberRepository _subscriberRepository;

    public NewsLetterController(ISubscriberRepository subscriberRepository)
    {
        _subscriberRepository = subscriberRepository;
    }

    public async Task<IActionResult> Subscribe(string email)
    {
        var subscription = await _subscriberRepository.SubscribeAsync(email);
        if (!subscription)
            return Content("Đã xảy ra lỗi khi đăng ký với email!");

        return Redirect(Request.Headers["Referer"].ToString());
    }

    public async Task<IActionResult> Unsubscribe(string email)
    {
        await _subscriberRepository.UnsubscribeAsync(email, "Không có nhu cầu nữa", true);

        return RedirectToAction("Index", "Blog");
    }
}