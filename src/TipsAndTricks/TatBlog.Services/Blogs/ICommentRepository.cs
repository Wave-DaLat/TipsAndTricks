using TatBlog.Core.Constracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;

namespace TatBlog.Services.Blogs;

public interface ICommentRepository
{
    Task<IPagedList<Comment>> GetCommentByPostIdAsync(int postId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);

    Task<IPagedList<Comment>> GetCommentByQueryAsync(CommentQuery query, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);

    Task<IPagedList<Comment>> GetCommentByQueryAsync(CommentQuery query, IPagingParams pagingParams, CancellationToken cancellationToken = default);

    Task<IPagedList<T>> GetCommentByQueryAsync<T>(CommentQuery query, IPagingParams pagingParams, Func<IQueryable<Comment>, IQueryable<T>> mapper, CancellationToken cancellationToken = default);

    Task<bool> AddCommentAsync(Comment comment, CancellationToken cancellationToken = default);

    Task<bool> DeleteCommentByIdAsync(int? id, CancellationToken cancellationToken = default);

    Task ChangeCommentStatusAsync(int id, CancellationToken cancellationToken = default);
}