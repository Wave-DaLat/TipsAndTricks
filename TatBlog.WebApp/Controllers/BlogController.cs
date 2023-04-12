using Microsoft.AspNetCore.Mvc;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Controllers;

public class BlogController : Controller
{
    private readonly IBlogRepository _blogRepository;
    private readonly ITagRepository _tagRepository;
    private readonly ICommentRepository _commentRepository;

    public BlogController(IBlogRepository blogRepository, ITagRepository tagRepository, ICommentRepository commentRepository)
    {
        _blogRepository = blogRepository;
        _tagRepository = tagRepository;
        _commentRepository = commentRepository;
    }

    public async Task<IActionResult> Index(
                              string keyword = null,
                              [FromQuery(Name = "p")] int pageNumber = 1,
                              [FromQuery(Name = "ps")] int pageSize = 10
                              )
    {
        // Tạo đối tượng chứa các điều kiện truy vấn
        var postQuery = new PostQuery
        {
            // Chỉ lấy những bài viết có trạng thái Published
            PublishedOnly = true,

            // Tìm kiếm bài viết theo tiêu đề
            Keyword = keyword
        };

        // Truy vấn các bài viết theo điều kiện đã tạo
        var postsList = await _blogRepository.GetPostByQueryAsync(postQuery, pageNumber, pageSize);

        // Lưu lại điều kiện truy vấn để hiển thị trong View
        ViewData["PostQuery"] = postQuery;
        ViewData["PagerQuery"] = new PagerQuery
        {
            Area = "",
            Controller = "Blog",
            Action = "Index",
        };

        return View(postsList);
    }

    public async Task<IActionResult> Category(
                              string slug = null)
    {
        if (slug == null) return NotFound();

        var postQuery = new PostQuery
        {
            CategorySlug = slug
        };

        var posts = await _blogRepository.GetPostByQueryAsync(postQuery);

        return View(posts);
    }

    public async Task<IActionResult> Author(
                              string slug = null)
    {
        if (slug == null) return NotFound();

        var postQuery = new PostQuery
        {
            AuthorSlug = slug
        };

        var posts = await _blogRepository.GetPostByQueryAsync(postQuery);

        return View(posts);
    }

    public async Task<IActionResult> Tag(
                              string slug = null)
    {
        if (slug == null) return NotFound();

        var postQuery = new PostQuery
        {
            TagSlug = slug
        };

        var posts = await _blogRepository.GetPostByQueryAsync(postQuery);
        var tag = await _tagRepository.GetTagBySlugAsync(slug);

        ViewData["Tag"] = tag;

        return View(posts);
    }

    public async Task<IActionResult> Post(
                              int year = 2023,
                              int month = 1,
                              int day = 1,
                              string slug = null)
    {
        if (slug == null) return NotFound();

        var post = await _blogRepository.GetPostAsync(year, month, day, slug);

        if (post == null) return Content("Không tìm thấy bài viết nào");

        if (!post.Published)
        {
            ModelState.AddModelError("denied access", "Bài viết này không được phép truy cập");
            return View();
        }
        else
        {
            await _blogRepository.IncreaseViewCountAsync(post.Id);
        }

        ViewData["Comments"] = await _commentRepository.GetCommentByPostIdAsync(post.Id);

        return View(post);
    }

    public async Task<IActionResult> Archives(int year, int month)
    {
        PostQuery query = new PostQuery
        {
            Year = year,
            Month = month
        };

        var posts = await _blogRepository.GetPostByQueryAsync(query);

        ViewData["PostQuery"] = query;

        return View(posts);
    }

    public IActionResult About() => View();

    public IActionResult Contact() => View();

    public IActionResult Rss() => View("Nội dung sẽ được cập nhập");

    [HttpPost]
    public async Task<IActionResult> Comment(int postId, string commentUsername, string commentContent)
    {
        Comment comment = new Comment
        {
            UserName = commentUsername,
            Content = commentContent,
            PostDate = DateTime.Now,
            Censored = false,
            PostID = postId
        };

        var added = await _commentRepository.AddCommentAsync(comment);

        if (!added)
        {
            return Content("Không thể tải lên bình luận của bạn! Hãy thử lại thao tác này");
        }

        return Redirect(Request.Headers["Referer"].ToString());
    }
}