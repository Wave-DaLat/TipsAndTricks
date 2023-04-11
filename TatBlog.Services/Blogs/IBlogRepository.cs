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

    Task<Tag> GetTagBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<IList<TagItem>> GetTagListWithPostCountAsync(CancellationToken cancellationToken = default);

    Task DeleteTagByIdAsync(int? id, CancellationToken cancellationToken = default);

    Task<Category> GetCategoryBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<Category> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default);

    Task AddOrUpdateCategoryAsync(Category category, CancellationToken cancellationToken = default);

    Task DeleteCategoryByIdAsync(int? id, CancellationToken cancellationToken = default);

    Task<bool> CheckCategorySlugExisted(string slug);

    Task<IList<PostInMonthItem>> CountPostInMonthAsync(int monthCount, CancellationToken cancellationToken = default);

    Task<Post> GetPostByIdAsync(int id, CancellationToken cancellationToken = default);

    Task AddOrUpdatePostAsync(Post post, CancellationToken cancellationToken = default);

    Task ChangePostStatusAsync(int id, CancellationToken cancellationToken = default);

    Task<IList<Post>> GetRandomPostAsync(int n, CancellationToken cancellationToken = default);

    Task<Author> GetAuthorByIdAsync(int id, CancellationToken cancellationToken);

    Task<Author> GetAuthorBySlugAsync(string slug, CancellationToken cancellationToken);

    Task<IPagedList<AuthorItem>> GetAuthorsAsync(IPagingParams pagingParams, CancellationToken cancellationToken = default);

    Task AddOrUpdateAuthorAsync(Author author, CancellationToken cancellationToken = default);

    Task<IList<Author>> Find_N_MostPostByAuthorAsync(int n, CancellationToken cancellationToken = default);
}
