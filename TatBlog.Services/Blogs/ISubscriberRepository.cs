using TatBlog.Core.Constracts;
using TatBlog.Core.Entities;

namespace TatBlog.Services.Blogs;

public interface ISubscriberRepository
{
    Task SubscribeAsync(string email);

    Task UnsubscribeAsync(string email, string reason, bool voluntary);

    Task BlockSubscriberAsync(int id, string reason, string notes);

    Task DeleteSubscriberAsync(int id);

    Task<Subscriber> GetSubscriberByIdAsync(int id);

    Task<Subscriber> GetSubscriberByEmailAsync(string email);

    Task<IPagedList<Subscriber>> SearchSubscribersAsync(IPagingParams pagingParams,
      string keyword, bool unsubscribed, bool involuntary);
}