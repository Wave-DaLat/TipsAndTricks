using TatBlog.Core.Constracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;

namespace TatBlog.Services.Blogs;

public interface IBlogRepository
{
    Task<Post> GetPostAsync(int year, int month, int day, string slug, CancellationToken cancellationToken = default);

    Task<Post> GetCachedPostAsync(int year, int month, int day, string slug, CancellationToken cancellationToken = default);

    Task<Post> GetPostByIdAsync(int id, bool published = false, CancellationToken cancellationToken = default);

    Task<Post> GetCachedPostByIdAsync(int id, bool published = false, CancellationToken cancellationToken = default);

    Task<IList<Post>> GetPopularArticlesAsync(int limit, CancellationToken cancellationToken = default);

    Task<IList<Post>> GetRandomPostAsync(int limit, CancellationToken cancellationToken = default);

    Task<IList<DateItem>> GetArchivesPostAsync(int limit, CancellationToken cancellationToken = default);

    Task<IPagedList<Post>> GetPostByQueryAsync(PostQuery query, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);

    Task<IPagedList<Post>> GetPostByQueryAsync(PostQuery query, IPagingParams pagingParams, CancellationToken cancellationToken = default);

    Task<IPagedList<T>> GetPostByQueryAsync<T>(PostQuery query, IPagingParams pagingParams, Func<IQueryable<Post>, IQueryable<T>> mapper, CancellationToken cancellationToken = default);

    Task<bool> AddOrUpdatePostAsync(Post post, IEnumerable<string> tags, CancellationToken cancellationToken = default);

    Task<bool> DeletePostByIdAsync(int? id, CancellationToken cancellationToken = default);

    Task<bool> IsPostSlugExistedAsync(int postId, string slug, CancellationToken cancellationToken = default);

    Task IncreaseViewCountAsync(int postId, CancellationToken cancellationToken = default);

    Task ChangePostStatusAsync(int id, CancellationToken cancellationToken = default);

    Task<IList<PostInMonthItem>> CountPostInMonthAsync(int monthCount, CancellationToken cancellationToken = default);
}