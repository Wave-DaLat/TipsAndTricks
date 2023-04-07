using TatBlog.Core.Constracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;

namespace TatBlog.Services.Blogs;

public interface IBlogRepository
{
    Task<IList<Post>> GetPopularArticlesAsync(int numPosts, CancellationToken cancellationToken = default);

    Task IncreaseViewCountAsync(int postId, CancellationToken cancellationToken = default);

    Task<IList<Post>> GetPostsAsync(PostQuery condition, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    Task<Post> GetPostAsync(int year, int month, string slug, CancellationToken cancellationToken = default);

    Task<int> CountPostsAsync(PostQuery condition, CancellationToken cancellationToken = default);

    Task<IList<MonthlyPostCountItem>> CountMonthlyPostsAsync(int numMonths, CancellationToken cancellationToken = default);

    Task<Category> GetCategoryAsync(string slug, CancellationToken cancellationToken = default);

    Task<Category> GetCategoryByIdAsync(int categoryId);

    Task<IList<CategoryItem>> GetCategoriesAsync(bool showOnMenu = false, CancellationToken cancellationToken = default);

    Task<IPagedList<CategoryItem>> GetPagedCategoriesAsync(IPagingParams pagingParams, CancellationToken cancellationToken = default);

    Task<Category> CreateOrUpdateCategoryAsync(Category category, CancellationToken cancellationToken = default);

    Task<bool> IsCategorySlugExistedAsync(int categoryId, string categorySlug, CancellationToken cancellationToken = default);

    Task<bool> DeleteCategoryAsync(int categoryId, CancellationToken cancellationToken = default);

    Task<Tag> GetTagAsync(string slug, CancellationToken cancellationToken = default);

    Task<IList<TagItem>> GetTagsAsync(CancellationToken cancellationToken = default);

    Task<IPagedList<TagItem>> GetPagedTagsAsync(IPagingParams pagingParams, CancellationToken cancellationToken = default);

    Task<bool> DeleteTagAsync(int tagId, CancellationToken cancellationToken = default);

    Task<bool> CreateOrUpdateTagAsync(Tag tag, CancellationToken cancellationToken = default);

    Task<Post> GetPostAsync(string slug, CancellationToken cancellationToken = default);

    Task<Post> GetPostByIdAsync(int postId, bool includeDetails = false, CancellationToken cancellationToken = default);

    Task<bool> TogglePublishedFlagAsync(int postId, CancellationToken cancellationToken = default);

    Task<IList<Post>> GetRandomArticlesAsync(int numPosts, CancellationToken cancellationToken = default);

    Task<IPagedList<Post>> GetPagedPostsAsync(PostQuery condition, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);

    Task<IPagedList<T>> GetPagedPostsAsync<T>(PostQuery condition, IPagingParams pagingParams, Func<IQueryable<Post>, IQueryable<T>> mapper);

    Task<Post> CreateOrUpdatePostAsync(Post post, IEnumerable<string> tags, CancellationToken cancellationToken = default);

    Task<bool> IsPostSlugExistedAsync(int postId, string slug, CancellationToken cancellationToken = default);
}
