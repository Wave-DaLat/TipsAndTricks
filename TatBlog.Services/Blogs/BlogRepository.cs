using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TatBlog.Core.Constracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Services.Extensions;

namespace TatBlog.Services.Blogs;

public class BlogRepository : IBlogRepository
{
    private readonly BlogDbContext _context;

    public BlogRepository(BlogDbContext context)
    {
        _context = context;
    }

    public async Task<IList<Post>> GetPopularArticlesAsync(
        int numPosts, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>()
            .Include(x => x.Author)
            .Include(x => x.Category)
            .OrderByDescending(p => p.ViewCount)
            .Take(numPosts)
            .ToListAsync(cancellationToken);
    }

    public async Task IncreaseViewCountAsync(
        int postId,
        CancellationToken cancellationToken = default)
    {
        await _context.Set<Post>()
            .Where(x => x.Id == postId)
            .ExecuteUpdateAsync(p =>
                    p.SetProperty(x => x.ViewCount, x => x.ViewCount + 1),
                cancellationToken);
    }

    public async Task<IList<Post>> GetPostsAsync(
        PostQuery condition,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await FilterPosts(condition)
            .OrderByDescending(x => x.PostedDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<Post> GetPostAsync(
        int year,
        int month,
        string slug,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Post> postsQuery = _context.Set<Post>()
            .Include(x => x.Category)
            .Include(x => x.Author);

        if (year > 0)
        {
            postsQuery = postsQuery.Where(x => x.PostedDate.Year == year);
        }

        if (month > 0)
        {
            postsQuery = postsQuery.Where(x => x.PostedDate.Month == month);
        }

        if (!string.IsNullOrWhiteSpace(slug))
        {
            postsQuery = postsQuery.Where(x => x.UrlSlug == slug);
        }

        return await postsQuery.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountPostsAsync(
        PostQuery condition, CancellationToken cancellationToken = default)
    {
        return await FilterPosts(condition).CountAsync(cancellationToken: cancellationToken);
    }

    public async Task<IList<MonthlyPostCountItem>> CountMonthlyPostsAsync(
        int numMonths, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>()
            .GroupBy(x => new { x.PostedDate.Year, x.PostedDate.Month })
            .Select(g => new MonthlyPostCountItem()
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                PostCount = g.Count(x => x.Published)
            })
            .OrderByDescending(x => x.Year)
            .ThenByDescending(x => x.Month)
            .ToListAsync(cancellationToken);
    }

    public async Task<Category> GetCategoryAsync(
        string slug, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Category>()
            .FirstOrDefaultAsync(x => x.UrlSlug == slug, cancellationToken);
    }

    public async Task<Category> GetCategoryByIdAsync(int categoryId)
    {
        return await _context.Set<Category>().FindAsync(categoryId);
    }

    public async Task<IList<CategoryItem>> GetCategoriesAsync(
        bool showOnMenu = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Category> categories = _context.Set<Category>();

        if (showOnMenu)
        {
            categories = categories.Where(x => x.ShowOnMenu);
        }

        return await categories
            .OrderBy(x => x.Name)
            .Select(x => new CategoryItem()
            {
                Id = x.Id,
                Name = x.Name,
                UrlSlug = x.UrlSlug,
                Description = x.Description,
                ShowOnMenu = x.ShowOnMenu,
                PostCount = x.Posts.Count(p => p.Published)
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IPagedList<CategoryItem>> GetPagedCategoriesAsync(
        IPagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var tagQuery = _context.Set<Category>()
            .Select(x => new CategoryItem()
            {
                Id = x.Id,
                Name = x.Name,
                UrlSlug = x.UrlSlug,
                Description = x.Description,
                ShowOnMenu = x.ShowOnMenu,
                PostCount = x.Posts.Count(p => p.Published)
            });

        return await tagQuery.ToPagedListAsync(pagingParams, cancellationToken);
    }

    public async Task<Category> CreateOrUpdateCategoryAsync(
        Category category, CancellationToken cancellationToken = default)
    {
        if (category.Id > 0)
        {
            _context.Set<Category>().Update(category);
        }
        else
        {
            _context.Set<Category>().Add(category);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return category;
    }

    public async Task<bool> IsCategorySlugExistedAsync(
        int categoryId, string categorySlug,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Category>()
            .AnyAsync(x => x.Id != categoryId && x.UrlSlug == categorySlug, cancellationToken);
    }

    public async Task<bool> DeleteCategoryAsync(
        int categoryId, CancellationToken cancellationToken = default)
    {
        var category = await _context.Set<Category>().FindAsync(categoryId);

        if (category is null) return false;

        _context.Set<Category>().Remove(category);
        var rowsCount = await _context.SaveChangesAsync(cancellationToken);

        return rowsCount > 0;
    }


    public async Task<Tag> GetTagAsync(
        string slug, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Tag>()
            .FirstOrDefaultAsync(x => x.UrlSlug == slug, cancellationToken);
    }

    public async Task<IList<TagItem>> GetTagsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Tag>()
            .OrderBy(x => x.Name)
            .Select(x => new TagItem()
            {
                Id = x.Id,
                Name = x.Name,
                UrlSlug = x.UrlSlug,
                Description = x.Description,
                PostCount = x.Posts.Count(p => p.Published)
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IPagedList<TagItem>> GetPagedTagsAsync(
        IPagingParams pagingParams, CancellationToken cancellationToken = default)
    {
        var tagQuery = _context.Set<Tag>()
            .OrderBy(x => x.Name)
            .Select(x => new TagItem()
            {
                Id = x.Id,
                Name = x.Name,
                UrlSlug = x.UrlSlug,
                Description = x.Description,
                PostCount = x.Posts.Count(p => p.Published)
            });

        return await tagQuery.ToPagedListAsync(pagingParams, cancellationToken);
    }

    public async Task<bool> DeleteTagAsync(
        int tagId, CancellationToken cancellationToken = default)
    {
        //var tag = await _context.Set<Tag>().FindAsync(tagId);

        //if (tag == null) return false;

        //_context.Set<Tag>().Remove(tag);
        //return await _context.SaveChangesAsync(cancellationToken) > 0;

        return await _context.Set<Tag>()
            .Where(x => x.Id == tagId)
            .ExecuteDeleteAsync(cancellationToken) > 0;
    }

    public async Task<bool> CreateOrUpdateTagAsync(
        Tag tag, CancellationToken cancellationToken = default)
    {
        if (tag.Id > 0)
        {
            _context.Set<Tag>().Update(tag);
        }
        else
        {
            _context.Set<Tag>().Add(tag);
        }

        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }


    public async Task<Post> GetPostAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        var postQuery = new PostQuery()
        {
            PublishedOnly = false,
            TitleSlug = slug
        };

        return await FilterPosts(postQuery).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Post> GetPostByIdAsync(
        int postId, bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        if (!includeDetails)
        {
            return await _context.Set<Post>().FindAsync(postId);
        }

        return await _context.Set<Post>()
            .Include(x => x.Category)
            .Include(x => x.Author)
            .Include(x => x.Tags)
            .FirstOrDefaultAsync(x => x.Id == postId, cancellationToken);
    }

    public async Task<bool> TogglePublishedFlagAsync(
        int postId, CancellationToken cancellationToken = default)
    {
        var post = await _context.Set<Post>().FindAsync(postId);

        if (post is null) return false;

        post.Published = !post.Published;
        await _context.SaveChangesAsync(cancellationToken);

        return post.Published;
    }

    public async Task<IList<Post>> GetRandomArticlesAsync(
        int numPosts, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>()
            .OrderBy(x => Guid.NewGuid())
            .Take(numPosts)
            .ToListAsync(cancellationToken);
    }

    public async Task<IPagedList<Post>> GetPagedPostsAsync(
        PostQuery condition,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        return await FilterPosts(condition).ToPagedListAsync(
            pageNumber, pageSize,
            nameof(Post.PostedDate), "DESC",
            cancellationToken);
    }

    public async Task<IPagedList<T>> GetPagedPostsAsync<T>(
        PostQuery condition,
        IPagingParams pagingParams,
        Func<IQueryable<Post>, IQueryable<T>> mapper)
    {
        var posts = FilterPosts(condition);
        var projectedPosts = mapper(posts);

        return await projectedPosts.ToPagedListAsync(pagingParams);
    }

    public async Task<Post> CreateOrUpdatePostAsync(
        Post post, IEnumerable<string> tags,
        CancellationToken cancellationToken = default)
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
                Slug = x //.GenerateSlug()
            })
            .GroupBy(x => x.Slug)
            .ToDictionary(g => g.Key, g => g.First().Name);


        foreach (var kv in validTags)
        {
            if (post.Tags.Any(x => string.Compare(x.UrlSlug, kv.Key, StringComparison.InvariantCultureIgnoreCase) == 0)) continue;

            var tag = await GetTagAsync(kv.Key, cancellationToken) ?? new Tag()
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

        await _context.SaveChangesAsync(cancellationToken);

        return post;
    }

    public async Task<bool> IsPostSlugExistedAsync(
        int postId, string slug, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>()
            .AnyAsync(x => x.Id != postId && x.UrlSlug == slug, cancellationToken);
    }

    private IQueryable<Post> FilterPosts(PostQuery condition)
    {
        IQueryable<Post> posts = _context.Set<Post>()
            .Include(x => x.Category)
            .Include(x => x.Author)
            .Include(x => x.Tags);

        if (condition.PublishedOnly)
        {
            posts = posts.Where(x => x.Published);
        }

        if (condition.NotPublished)
        {
            posts = posts.Where(x => !x.Published);
        }

        if (condition.CategoryId > 0)
        {
            posts = posts.Where(x => x.CategoryId == condition.CategoryId);
        }

        if (!string.IsNullOrWhiteSpace(condition.CategorySlug))
        {
            posts = posts.Where(x => x.Category.UrlSlug == condition.CategorySlug);
        }

        if (condition.AuthorId > 0)
        {
            posts = posts.Where(x => x.AuthorId == condition.AuthorId);
        }

        if (!string.IsNullOrWhiteSpace(condition.AuthorSlug))
        {
            posts = posts.Where(x => x.Author.UrlSlug == condition.AuthorSlug);
        }

        if (!string.IsNullOrWhiteSpace(condition.TagSlug))
        {
            posts = posts.Where(x => x.Tags.Any(t => t.UrlSlug == condition.TagSlug));
        }

        if (!string.IsNullOrWhiteSpace(condition.Keyword))
        {
            posts = posts.Where(x => x.Title.Contains(condition.Keyword) ||
                                     x.ShortDescription.Contains(condition.Keyword) ||
                                     x.Description.Contains(condition.Keyword) ||
                                     x.Category.Name.Contains(condition.Keyword) ||
                                     x.Tags.Any(t => t.Name.Contains(condition.Keyword)));
        }

        if (condition.Year > 0)
        {
            posts = posts.Where(x => x.PostedDate.Year == condition.Year);
        }

        if (condition.Month > 0)
        {
            posts = posts.Where(x => x.PostedDate.Month == condition.Month);
        }

        if (!string.IsNullOrWhiteSpace(condition.TitleSlug))
        {
            posts = posts.Where(x => x.UrlSlug == condition.TitleSlug);
        }

        return posts;

        //// Compact version
        //return _context.Set<Post>()
        //	.Include(x => x.Category)
        //	.Include(x => x.Author)
        //	.Include(x => x.Tags)
        //	.WhereIf(condition.PublishedOnly, x => x.Published)
        //	.WhereIf(condition.NotPublished, x => !x.Published)
        //	.WhereIf(condition.CategoryId > 0, x => x.CategoryId == condition.CategoryId)
        //	.WhereIf(!string.IsNullOrWhiteSpace(condition.CategorySlug), x => x.Category.UrlSlug == condition.CategorySlug)
        //	.WhereIf(condition.AuthorId > 0, x => x.AuthorId == condition.AuthorId)
        //	.WhereIf(!string.IsNullOrWhiteSpace(condition.AuthorSlug), x => x.Author.UrlSlug == condition.AuthorSlug)
        //	.WhereIf(!string.IsNullOrWhiteSpace(condition.TagSlug), x => x.Tags.Any(t => t.UrlSlug == condition.TagSlug))
        //	.WhereIf(!string.IsNullOrWhiteSpace(condition.Keyword), x => x.Title.Contains(condition.Keyword) ||
        //	                                                             x.ShortDescription.Contains(condition.Keyword) ||
        //	                                                             x.Description.Contains(condition.Keyword) ||
        //	                                                             x.Category.Name.Contains(condition.Keyword) ||
        //	                                                             x.Tags.Any(t => t.Name.Contains(condition.Keyword)))
        //	.WhereIf(condition.Year > 0, x => x.PostedDate.Year == condition.Year)
        //	.WhereIf(condition.Month > 0, x => x.PostedDate.Month == condition.Month)
        //	.WhereIf(!string.IsNullOrWhiteSpace(condition.TitleSlug), x => x.UrlSlug == condition.TitleSlug);
    }

    // ......

    public async Task<Tag> GetTagBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Tag>()
                                .Where(t => t.UrlSlug.Contains(slug))
                                .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IList<TagItem>> GetTagListWithPostCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<Tag>()
                                  .Include(t => t.Posts)
                                  .Select(x => new TagItem()
                                  {
                                      Id = x.Id,
                                      Name = x.Name,
                                      UrlSlug = x.UrlSlug,
                                      Description = x.Description,
                                      PostCount = x.Posts.Count()
                                  }).ToListAsync(cancellationToken);
    }
    public async Task DeleteTagByIdAsync(int? id, CancellationToken cancellationToken = default)
    {
        if (id == null || _context.Tags == null)
        {
            Console.WriteLine("Không có tag nào");
            return;
        }

        var tag = await _context.Set<Tag>().FindAsync(id);

        if (tag != null)
        {
            Tag tagContext = tag;
            _context.Tags.Remove(tagContext);
            await _context.SaveChangesAsync(cancellationToken);

            Console.WriteLine($"Đã xóa tag với id {id}");
        }
    }

    public async Task<Category> GetCategoryBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _context.Categories.FindAsync(slug, cancellationToken);

        // return await _context.Set<Category>()
        //                         .Where(c => c.UrlSlug.Contains(slug))
        //                         .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Category> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Category>().FindAsync(id);
    }

    public async Task AddOrUpdateCategoryAsync(Category category, CancellationToken cancellationToken = default)
    {
        if (category?.Id == null || _context.Categories == null)
        {
            await _context.Categories.AddAsync(category, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return;
        }

        var cat = await _context.Categories.FirstOrDefaultAsync(m => m.Id == category.Id);
        if (cat == null)
        {
            Console.WriteLine("Không có category nào để sửa");
            return;
        }

        cat.Name = category.Name;
        cat.Description = category.Description;
        cat.UrlSlug = category.UrlSlug;
        cat.ShowOnMenu = category.ShowOnMenu;

        _context.Attach(cat).State = EntityState.Modified;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteCategoryByIdAsync(int? id, CancellationToken cancellationToken = default)
    {
        if (id == null || _context.Tags == null)
        {
            Console.WriteLine("Không có chuyên mục nào");
            return;
        }

        var tag = await _context.Set<Tag>().FindAsync(id);

        if (tag != null)
        {
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync(cancellationToken);

            Console.WriteLine($"Đã xóa chuyên mục với id {id}");
        }
    }

    public async Task<bool> CheckCategorySlugExisted(string slug)
    {
        return await _context.Set<Category>().AnyAsync(c => c.UrlSlug.Equals(slug));
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

    public async Task<Post> GetPostByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>().FindAsync(id);
    }

    public async Task AddOrUpdatePostAsync(Post post, CancellationToken cancellationToken = default)
    {
        if (post?.Id == null || _context.Posts == null)
        {
            await _context.Posts.AddAsync(post, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return;
        }

        var postGet = await _context.Posts.FirstOrDefaultAsync(m => m.Id == post.Id);
        if (postGet == null)
        {
            Console.WriteLine("Không có post nào để sửa");
            return;
        }

        postGet.Title = post.Title;
        postGet.Description = post.Description;
        postGet.UrlSlug = post.UrlSlug;
        postGet.Published = post.Published;

        _context.Attach(postGet).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task ChangePostStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        var post = await _context.Posts.FindAsync(id);

        post.Published = !post.Published;

        _context.Attach(post).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task<IList<Post>> GetRandomPostAsync(int n, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>().OrderBy(p => Guid.NewGuid()).Take(n).ToListAsync(cancellationToken);
    }

    public async Task<Author> GetAuthorByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Authors.FindAsync(id, cancellationToken);
    }

    public async Task<Author> GetAuthorBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        return await _context.Authors.FindAsync(slug, cancellationToken);
    }

    public async Task<IPagedList<AuthorItem>> GetAuthorsAsync(IPagingParams pagingParams, CancellationToken cancellationToken = default)
    {
        var tagQuery = _context.Set<Author>()
                                  .Select(x => new AuthorItem()
                                  {
                                      Id = x.Id,
                                      FullName = x.FullName,
                                      UrlSlug = x.UrlSlug,
                                      ImageUrl = x.ImageUrl,
                                      JoinedDate = x.JoinedDate,
                                      Notes = x.Notes,
                                      PostCount = x.Posts.Count(p => p.Published)
                                  });

        return await tagQuery.ToPagedListAsync(pagingParams, cancellationToken);
    }

    public async Task AddOrUpdateAuthorAsync(Author author, CancellationToken cancellationToken = default)
    {
        if (author?.Id == null || _context.Authors == null)
        {
            await _context.Authors.AddAsync(author, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return;
        }

        var aut = await _context.Authors.FirstOrDefaultAsync(a => a.Id == author.Id);
        if (aut == null)
        {
            Console.WriteLine("Không có author nào để sửa");
            return;
        }

        aut.FullName = author.FullName;
        aut.UrlSlug = author.UrlSlug;
        aut.JoinedDate = author.JoinedDate;
        aut.Email = author.Email;
        aut.Notes = author.Notes;

        _context.Attach(aut).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task<IList<Author>> Find_N_MostPostByAuthorAsync(int n, CancellationToken cancellationToken = default)
    {
        IQueryable<Author> authorsQuery = _context.Set<Author>();
        IQueryable<Post> postsQuery = _context.Set<Post>();

        return await authorsQuery.Join(postsQuery, a => a.Id, p => p.AuthorId,
                                      (author, post) => new
                                      {
                                          author.Id
                                      })
                                 .GroupBy(x => x.Id)
                                 .Select(x => new
                                 {
                                     AuthorId = x.Key,
                                     Count = x.Count()
                                 })
                                 .OrderByDescending(x => x.Count)
                                 .Take(n)
                                 .Join(authorsQuery, a => a.AuthorId, a2 => a2.Id,
                                  (preQuery, author) => new Author
                                  {
                                      Id = author.Id,
                                      FullName = author.FullName,
                                      UrlSlug = author.UrlSlug,
                                      ImageUrl = author.ImageUrl,
                                      JoinedDate = author.JoinedDate,
                                      Notes = author.Notes,
                                  }).ToListAsync();
    }
}