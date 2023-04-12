using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;

using TatBlog.Core.Constracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Services.Extensions;

namespace TatBlog.Services.Blogs;

public class BlogRepository : IBlogRepository
{
    private readonly BlogDbContext _context;
    private readonly ITagRepository _tagRepository;
    private readonly IMemoryCache _memoryCache;

    public BlogRepository(BlogDbContext dbContext, ITagRepository tagRepository, IMemoryCache memoryCache)
    {
        _context = dbContext;
        _tagRepository = tagRepository;
        _memoryCache = memoryCache;
    }

    public async Task<Post> GetPostAsync(int year, int month, int day, string slug, CancellationToken cancellationToken = default)
    {
        IQueryable<Post> postsQuery = _context.Set<Post>()
                                                  .Include(x => x.Category)
                                                  .Include(x => x.Author)
                                                  .Include(x => x.Tags);

        if (year > 0)
        {
            postsQuery = postsQuery.Where(x => x.PostedDate.Year == year);
        }

        if (month > 0)
        {
            postsQuery = postsQuery.Where(x => x.PostedDate.Month == month);
        }

        if (day > 0)
        {
            postsQuery = postsQuery.Where(x => x.PostedDate.Day == day);
        }

        if (!string.IsNullOrWhiteSpace(slug))
        {
            postsQuery = postsQuery.Where(x => x.UrlSlug == slug);
        }

        return await postsQuery.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Post> GetCachedPostAsync(int year, int month, int day, string slug, CancellationToken cancellationToken = default)
    {
        return await _memoryCache.GetOrCreateAsync(
            $"post.{year}-{month}-{day}-{slug}",
            async (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return await GetCachedPostAsync(year, month, day, slug, cancellationToken);
            });
    }

    public async Task<Post> GetPostByIdAsync(int id, bool published = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Post> postQuery = _context.Set<Post>()
                                                                                        .Include(p => p.Category)
                                                                                        .Include(p => p.Author)
                                                                                        .Include(p => p.Tags);

        if (published)
        {
            postQuery = postQuery.Where(x => x.Published);
        }

        return await postQuery.Where(p => p.Id.Equals(id))
                              .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Post> GetCachedPostByIdAsync(int id, bool published = false, CancellationToken cancellationToken = default)
    {
        return await _memoryCache.GetOrCreateAsync(
            $"post.by-id.{id}-{published}",
            async (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return await GetPostByIdAsync(id, published, cancellationToken);
            });
    }

    public async Task<IList<Post>> GetPopularArticlesAsync(int limit, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>()
                                 .Include(x => x.Author)
                                 .Include(x => x.Category)
                                 .Include(x => x.Tags)
                                 .OrderByDescending(p => p.ViewCount)
                                 .Take(limit)
                                 .ToListAsync(cancellationToken);
    }

    public async Task<IList<Post>> GetRandomPostAsync(int limit, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>()
                                 .Include(x => x.Author)
                                 .Include(x => x.Category)
                                 .Include(x => x.Tags)
                                 .OrderBy(p => Guid.NewGuid())
                                 .Take(limit)
                                 .ToListAsync(cancellationToken);
    }

    public async Task<IList<DateItem>> GetArchivesPostAsync(int limit, CancellationToken cancellationToken = default)
    {
        var lastestMonths = await GetLatestMonthList(limit);

        return await Task.FromResult(_context.Set<Post>().AsEnumerable()
                                                            .GroupBy(p => new
                                                            {
                                                                p.PostedDate.Month,
                                                                p.PostedDate.Year
                                                            })
                                                            .Join(lastestMonths, d => d.Key.Month, m => m.Month,
                                                            (postDate, monthGet) => new DateItem
                                                            {
                                                                Month = postDate.Key.Month,
                                                                MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(postDate.Key.Month),
                                                                Year = postDate.Key.Year,
                                                                PostCount = postDate.Count()
                                                            }).ToList());
    }

    public async Task<IPagedList<Post>> GetPostByQueryAsync(PostQuery query, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        return await FilterPosts(query).ToPagedListAsync(
                                pageNumber,
                                pageSize,
                                nameof(Post.PostedDate),
                                "DESC",
                                cancellationToken);
    }

    public async Task<IPagedList<Post>> GetPostByQueryAsync(PostQuery query, IPagingParams pagingParams, CancellationToken cancellationToken = default)
    {
        return await FilterPosts(query).ToPagedListAsync(
                                        pagingParams,
                                        cancellationToken);
    }

    public async Task<IPagedList<T>> GetPostByQueryAsync<T>(PostQuery query, IPagingParams pagingParams, Func<IQueryable<Post>, IQueryable<T>> mapper, CancellationToken cancellationToken = default)
    {
        IQueryable<T> result = mapper(FilterPosts(query));

        return await result.ToPagedListAsync(pagingParams, cancellationToken);
    }

    public async Task<bool> AddOrUpdatePostAsync(Post post, IEnumerable<string> tags, CancellationToken cancellationToken = default)
    {
        if (post.Id > 0)
        {
            await _context.Entry(post).Collection(x => x.Tags).LoadAsync(cancellationToken);
        }
        else
        {
            post.Tags = new List<Tag>();
        }

        var validTags = tags.Where(x => !string.IsNullOrWhiteSpace(x))
          .Select(x => new
          {
              Name = x,
              Slug = x.GenerateSlug()
          })
          .GroupBy(x => x.Slug)
          .ToDictionary(g => g.Key, g => g.First().Name);

        foreach (var kv in validTags)
        {
            if (post.Tags.Any(x => string.Compare(x.UrlSlug, kv.Key, StringComparison.InvariantCultureIgnoreCase) == 0)) continue;

            var tag = await _tagRepository.GetTagBySlugAsync(kv.Key, cancellationToken) ?? new Tag()
            {
                Name = kv.Value,
                Description = kv.Value,
                UrlSlug = kv.Key
            };

            post.Tags.Add(tag);
        }

        post.Tags = post.Tags.Where(t => validTags.ContainsKey(t.UrlSlug)).ToList();

        if (post.Id > 0)
            _context.Update(post);
        else
            _context.Add(post);

        var result = await _context.SaveChangesAsync(cancellationToken);
        return result > 0;
    }

    public async Task<bool> DeletePostByIdAsync(int? id, CancellationToken cancellationToken = default)
    {
        var post = await _context.Set<Post>().FindAsync(id);

        if (post is null) return false;

        _context.Set<Post>().Remove(post);
        var rowsCount = await _context.SaveChangesAsync(cancellationToken);

        return rowsCount > 0;
    }

    public async Task<bool> IsPostSlugExistedAsync(int postId, string slug, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>()
                                 .AnyAsync(x => x.Id != postId && x.UrlSlug == slug, cancellationToken);
    }

    public async Task IncreaseViewCountAsync(int postId, CancellationToken cancellationToken = default)
    {
        await _context.Set<Post>()
                          .Where(x => x.Id == postId)
                          .ExecuteUpdateAsync(p => p.SetProperty(x => x.ViewCount, x => x.ViewCount + 1), cancellationToken);
    }

    public async Task ChangePostStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        var post = await _context.Posts.FindAsync(id);

        post.Published = !post.Published;

        _context.Attach(post).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task<IList<PostInMonthItem>> CountPostInMonthAsync(int monthCount, CancellationToken cancellationToken = default)
    {
        IQueryable<Post> postsQuery = _context.Set<Post>()
                                                      .OrderByDescending(p => p.PostedDate);

        var topDate = await postsQuery.Select(p => p.PostedDate).FirstOrDefaultAsync();
        var subDate = topDate.AddMonths(-monthCount);
        postsQuery = postsQuery.Where(x => x.PostedDate >= subDate);

        var result = from p in postsQuery
                     group p by new
                     {
                         p.PostedDate.Year,
                         p.PostedDate.Month
                     } into postCount
                     select new PostInMonthItem
                     {
                         Count = postCount.Count(),
                         Year = postCount.Key.Year.ToString(),
                         Month = postCount.Key.Month.ToString()
                     };

        return await result.ToListAsync(cancellationToken);
    }

    private IQueryable<Post> FilterPosts(PostQuery query)
    {
        IQueryable<Post> postsQuery = _context.Set<Post>()
                                                  .Include(p => p.Author)
                                                  .Include(p => p.Category)
                                                  .Include(p => p.Tags);

        if (query.PublishedOnly)
        {
            postsQuery = postsQuery.Where(x => x.Published);
        }

        if (query.NotPublished)
        {
            postsQuery = postsQuery.Where(x => !x.Published);
        }

        if (query.CategoryId > 0)
        {
            postsQuery = postsQuery.Where(x => x.CategoryId == query.CategoryId);
        }
        if (query.AuthorId > 0)
        {
            postsQuery = postsQuery.Where(x => x.AuthorId == query.AuthorId);
        }

        if (!string.IsNullOrWhiteSpace(query.AuthorSlug))
        {
            postsQuery = postsQuery.Where(x => x.Author.UrlSlug == query.AuthorSlug);
        }

        if (!string.IsNullOrWhiteSpace(query.CategorySlug))
        {
            postsQuery = postsQuery.Where(x => x.Category.UrlSlug == query.CategorySlug);
        }

        if (!string.IsNullOrWhiteSpace(query.TagSlug))
        {
            postsQuery = postsQuery.Where(x => x.Tags.Any(t => t.UrlSlug == query.TagSlug));
        }

        if (!string.IsNullOrWhiteSpace(query.PostSlug))
        {
            postsQuery = postsQuery.Where(x => x.UrlSlug == query.PostSlug);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            postsQuery = postsQuery.Where(x => x.Title.Contains(query.Keyword) ||
                         x.ShortDescription.Contains(query.Keyword) ||
                         x.Description.Contains(query.Keyword) ||
                         x.Category.Name.Contains(query.Keyword) ||
                         x.Author.FullName.Contains(query.Keyword) ||
                         x.Tags.Any(t => t.Name.Contains(query.Keyword)));
        }

        if (query.Year > 0)
        {
            postsQuery = postsQuery.Where(x => x.PostedDate.Year == query.Year);
        }

        if (query.Month > 0)
        {
            postsQuery = postsQuery.Where(x => x.PostedDate.Month == query.Month);
        }

        if (query.Day > 0)
        {
            postsQuery = postsQuery.Where(x => x.PostedDate.Day == query.Day);
        }

        query.GetTagListAsync();
        if (query.SelectedTag != null && query.SelectedTag.Count() > 0)
        {
            postsQuery = postsQuery.Where(p => query.SelectedTag.Intersect(p.Tags.Select(t => t.Name)).Count() > 0);
        }

        return postsQuery;
    }

    public async Task<IList<DateItem>> GetLatestMonthList(int limit)
    {
        return await Task.FromResult((from r in Enumerable.Range(1, 12) select DateTime.Now.AddMonths(limit - r))
                            .Select(x => new DateItem
                            {
                                Month = x.Month,
                                Year = x.Year
                            }).ToList());
    }
}
