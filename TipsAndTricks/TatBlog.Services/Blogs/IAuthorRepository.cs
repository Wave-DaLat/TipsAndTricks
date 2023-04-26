using TatBlog.Core.Constracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;

namespace TatBlog.Services.Blogs;

public interface IAuthorRepository
{
    Task<IList<AuthorItem>> GetAuthorsAsync(CancellationToken cancellationToken = default);

    Task<Author> GetAuthorByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Author> GetCachedAuthorByIdAsync(int authorId);

    Task<Author> GetAuthorBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<Author> GetCachedAuthorBySlugAsync(
        string slug, CancellationToken cancellationToken = default);

    Task<IPagedList<Author>> GetAuthorByQueryAsync(AuthorQuery query, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);

    Task<IPagedList<Author>> GetAuthorByQueryAsync(AuthorQuery query, IPagingParams pagingParams, CancellationToken cancellationToken = default);

    Task<IPagedList<T>> GetAuthorByQueryAsync<T>(AuthorQuery query, IPagingParams pagingParams, Func<IQueryable<Author>, IQueryable<T>> mapper, CancellationToken cancellationToken = default);

    Task<bool> AddOrUpdateAuthorAsync(Author author, CancellationToken cancellationToken = default);

    Task<bool> DeleteAuthorByIdAsync(int? id, CancellationToken cancellationToken = default);

    Task<IList<Author>> Find_N_PostsByAuthorAsync(int limit, CancellationToken cancellationToken = default);

    Task<bool> CheckAuthorSlugExisted(int id, string slug, CancellationToken cancellationToken = default);

    Task<bool> SetImageUrlAsync(
        int authorId, string imageUrl,
        CancellationToken cancellationToken = default);
}