using TatBlog.Core.Constracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;

namespace TatBlog.Services.Blogs;

public interface ISubscriberRepository
{
    Task<Subscriber> GetSubscriberByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Subscriber> GetCachedSubscriberByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Subscriber> GetSubscriberByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<Subscriber> GetCachedSubscriberByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<IPagedList<Subscriber>> GetSubscriberByQueryAsync(SubscriberQuery query, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);

    Task<IPagedList<Subscriber>> GetSubscriberByQueryAsync(SubscriberQuery query, IPagingParams pagingParams, CancellationToken cancellationToken = default);

    Task<IPagedList<T>> GetSubscriberByQueryAsync<T>(SubscriberQuery query, IPagingParams pagingParams, Func<IQueryable<Subscriber>, IQueryable<T>> mapper, CancellationToken cancellationToken = default);

    Task<bool> SubscribeAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> DeleteSubscriberAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> BlockSubscriberAsync(int id, string reason, string notes, CancellationToken cancellationToken = default);

    Task<bool> UnsubscribeAsync(string email, string reason, bool voluntary, CancellationToken cancellationToken = default);
}