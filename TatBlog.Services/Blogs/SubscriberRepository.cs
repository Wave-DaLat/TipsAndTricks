using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TatBlog.Core.Constracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Services.Blogs;
using TatBlog.Services.Extensions;
using TatBlog.Services.Media;

public class SubscriberRepository : ISubscriberRepository
{
    private readonly BlogDbContext _context;
    private readonly SendMailService _sendMailService;
    private readonly IMemoryCache _memoryCache;

    public SubscriberRepository(BlogDbContext dbContext, SendMailService sendMailService, IMemoryCache memoryCache)
    {
        _context = dbContext;
        _sendMailService = sendMailService;
        _memoryCache = memoryCache;
    }

    public async Task<Subscriber> GetSubscriberByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Subscriber>()
                                 .Where(s => s.SubscribeEmail.Equals(email))
                                 .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Subscriber> GetCachedSubscriberByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _memoryCache.GetOrCreateAsync(
            $"subscriber.by-email.{email}",
            async (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return await GetSubscriberByEmailAsync(email, cancellationToken);
            });
    }

    public async Task<Subscriber> GetSubscriberByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Subscriber>().FindAsync(id, cancellationToken);
    }

    public async Task<Subscriber> GetCachedSubscriberByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _memoryCache.GetOrCreateAsync(
            $"subscriber.by-id.{id}",
            async (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return await GetSubscriberByIdAsync(id, cancellationToken);
            });
    }

    public async Task<IPagedList<Subscriber>> GetSubscriberByQueryAsync(SubscriberQuery query, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        return await FilterSubscribers(query).ToPagedListAsync(
                                              pageNumber,
                                              pageSize,
                                              nameof(Subscriber.SubDated),
                                              "DESC",
                                              cancellationToken);
    }

    public async Task<IPagedList<Subscriber>> GetSubscriberByQueryAsync(SubscriberQuery query, IPagingParams pagingParams, CancellationToken cancellationToken = default)
    {
        return await FilterSubscribers(query).ToPagedListAsync(pagingParams, cancellationToken);
    }

    public async Task<IPagedList<T>> GetSubscriberByQueryAsync<T>(SubscriberQuery query, IPagingParams pagingParams, Func<IQueryable<Subscriber>, IQueryable<T>> mapper, CancellationToken cancellationToken = default)
    {
        IQueryable<T> result = mapper(FilterSubscribers(query));

        return await result.ToPagedListAsync(pagingParams, cancellationToken);
    }

    public async Task<bool> SubscribeAsync(string email, CancellationToken cancellationToken = default)
    {
        var subscriberExisted = await GetSubscriberByEmailAsync(email);

        if (subscriberExisted != null)
        {
            if (subscriberExisted.UnSubDated == null)
                return await Task.FromResult(false);

            subscriberExisted.UnSubDated = null;
            _context.Attach(subscriberExisted).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
        }

        MailContent mailContent = new MailContent
        {
            To = email,
            Subject = "Đăng ký theo dõi blog",
            Body = "<h1>Đăng ký thành công</h1><i>Cảm ơn bạn đã đăng ký theo dõi blog</i>"
        };

        Subscriber subscriber = new Subscriber
        {
            SubscribeEmail = email,
            SubDated = DateTime.Now
        };

        _context.Add(subscriber);

        await _sendMailService.SendEmailAsync(mailContent);
        var affects = await _context.SaveChangesAsync(cancellationToken);

        return affects > 0;
    }

    public async Task<bool> DeleteSubscriberAsync(int id, CancellationToken cancellationToken = default)
    {
        var subscriber = await _context.Set<Subscriber>().FindAsync(id);

        if (subscriber is null)
            return await Task.FromResult(false);

        _context.Set<Subscriber>().Remove(subscriber);
        var affects = await _context.SaveChangesAsync(cancellationToken);

        return affects > 0;
    }

    public async Task<bool> BlockSubscriberAsync(int id, string reason, string notes, CancellationToken cancellationToken = default)
    {
        var subscriber = await GetSubscriberByIdAsync(id);
        if (subscriber == null)
        {
            Console.WriteLine("Không có người đăng ký nào để chặn");
            return await Task.FromResult(false);
        }

        subscriber.CancelReason = reason;
        subscriber.AdminNotes = notes;
        subscriber.ForceLock = true;

        _context.Attach(subscriber).State = EntityState.Modified;
        var affects = await _context.SaveChangesAsync(cancellationToken);

        return affects > 0;
    }

    public async Task<bool> UnsubscribeAsync(string email, string reason, bool voluntary, CancellationToken cancellationToken = default)
    {
        var subscriber = await GetSubscriberByEmailAsync(email);
        if (subscriber == null)
        {
            Console.WriteLine("Không có người đăng ký này để hủy đăng ký");
            return await Task.FromResult(false);
        }

        subscriber.CancelReason = reason;
        subscriber.UnsubscribeVoluntary = true;
        subscriber.UnSubDated = DateTime.Now;

        _context.Attach(subscriber).State = EntityState.Modified;

        var result = await _context.SaveChangesAsync(cancellationToken);
        return result > 0;
    }

    private IQueryable<Subscriber> FilterSubscribers(SubscriberQuery query)
    {
        IQueryable<Subscriber> categoryQuery = _context.Set<Subscriber>();

        if (!string.IsNullOrWhiteSpace(query.Email))
        {
            categoryQuery = categoryQuery.Where(x => x.SubscribeEmail.Equals(query.Email));
        }

        if (query.ForceLock)
        {
            categoryQuery = categoryQuery.Where(x => x.ForceLock);
        }

        if (query.UnsubscribeVoluntary)
        {
            categoryQuery = categoryQuery.Where(x => x.UnsubscribeVoluntary);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            categoryQuery = categoryQuery.Where(x => x.CancelReason.Contains(query.Keyword) ||
                         x.AdminNotes.Contains(query.Keyword));
        }

        return categoryQuery;
    }
}