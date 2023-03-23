using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;

namespace TatBlog.Data.Seeder;

public class DataSeeder : IDataSeeder
{
    private readonly BlogDbContext _dbContext;

    public DataSeeder(BlogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Initialize()
    {
        _dbContext.Database.EnsureCreated();

        if (_dbContext.Posts.Any()) { return; }

        var authors = AddAuthors();
        var categories = AddCategories();
        var tags = AddTags();
        var posts = AddPosts(authors, categories, tags);
    }

    private IList<Author> AddAuthors()
    {
        var authors = new List<Author>()
        {
            new()
            {
                FullName = "Jason Mouth",
                UrlSlug = "jason-mouth",
                Email = "json@gmail.com",
                JoinedDate = new DateTime(2022, 10, 21)
            },
            new()
            {
                FullName = "Jessica Wonder",
                UrlSlug = "jessica-wonder",
                Email = "jessica665@motip.com",
                JoinedDate = new DateTime(2020, 4, 19)
            },
            new()
            {
                FullName = "Thomas Prim",
                UrlSlug = "thomas-prim",
                Email = "thomas@gmail.com",
                JoinedDate = new DateTime(2022, 3, 8)
            },
            new()
            {
                FullName = "Michael Jackson",
                UrlSlug = "michael-jackson",
                Email = "michael@gmail.com",
                JoinedDate = new DateTime(2019, 5, 20)
            },
            new()
            {
                FullName = "Isaac Newton",
                UrlSlug = "isaac-newton",
                Email = "isaac@gmail.com",
                JoinedDate = new DateTime(2022, 11, 28)
            }
        };

        _dbContext.Authors.AddRange(authors);
        _dbContext.SaveChanges();

        return authors;
    }

    private IList<Category> AddCategories()
    {
        var categories = new List<Category>()
        {
            new() { Name = ".NET Core", Description = ".NET Core", UrlSlug = "net-core" },
            new() { Name = "Architecture", Description = "Architecture", UrlSlug = "architecture" },
            new() { Name = "Messaging", Description = "Messaging", UrlSlug = "messaging" },
            new() { Name = "OPP", Description = "Object-Oriented Program", UrlSlug = "oop" },
            new() { Name = "Design Patterns", Description = "Design Patterns", UrlSlug = "design-patterns" },
            new() { Name = "Vue.js", Description = "Vue.js", UrlSlug = "vuejs" },
            new() { Name = "CLI", Description = "Command Line Interface", UrlSlug = "cli" },
            new() { Name = "React", Description = "React", UrlSlug = "react" },
            new() { Name = "NPM", Description = "Node Package Manager", UrlSlug = "npm" },
            new() { Name = "Node.js", Description = "Node.js", UrlSlug = "nodejs" }
        };

        _dbContext.Categories.AddRange(categories);
        _dbContext.SaveChanges();

        return categories;
    }

    private IList<Tag> AddTags()
    {
        var tags = new List<Tag>()
        {
            new() { Name = "Google", Description = "Google applications", UrlSlug = "google" },
            new() { Name = "ASP.NET MVC", Description = "ASP.NET MVC", UrlSlug = "aspnet-mvc" },
            new() { Name = "Razor Page", Description = "Razor Page", UrlSlug = "razor-page" },
            new() { Name = "Blazor", Description = "Blazor", UrlSlug = "blazor" },
            new() { Name = "Deep Learning", Description = "Deep Learning", UrlSlug = "deep-learning" },
            new() { Name = "Neural Network", Description = "Neural Network", UrlSlug = "neural-network" },
            new() { Name = "Xampp Control Panel", Description = "Xampp Control Panel", UrlSlug = "xampp" },
            new() { Name = "HyperText Markup Language", Description = "HyperText Markup Language", UrlSlug = "html" },
            new() { Name = "Cascading Style Sheets", Description = "Cascading Style Sheets", UrlSlug = "css" },
            new() { Name = "JavaScript", Description = "JavaScript", UrlSlug = "js" },
            new() { Name = "Ruby Programming Language", Description = "Ruby Programming Language", UrlSlug = "'ruby" },
            new() { Name = "Go Programming Language", Description = "Go Programming Language", UrlSlug = "golang" },
            new() { Name = "Back End", Description = "Back End", UrlSlug = "back-end" },
            new() { Name = "Front End", Description = "Front End", UrlSlug = "front-end" },
            new() { Name = "Full Stack", Description = "Full Stack", UrlSlug = "full-stack" },
            new() { Name = "Senior Developer", Description = "Senior Developer", UrlSlug = "senior" },
            new() { Name = "Junior Developer", Description = "Junior Developer", UrlSlug = "junior" },
            new() { Name = "Leader Developer", Description = "Leader Developer", UrlSlug = "leader" },
            new() { Name = "Developer", Description = "Developer", UrlSlug = "dev" },
            new() { Name = "Chat GPT", Description = "Chat GPT", UrlSlug = "chat-gpt" }
    };

        _dbContext.Tags.AddRange(tags);
        _dbContext.SaveChanges();

        return tags;
    }

    private IList<Post> AddPosts(
        IList<Author> authors,
        IList<Category> categories,
        IList<Tag> tags)
    {
        var posts = new List<Post>()
        {
            new()
            {
                Title = "ASP.NET Core Diagnostic Scenarios",
                ShortDescription = "David and friends has a great repos...",
                Description = "Here's a few great DON'T and DO examples...",
                Meta = "David and friends has a great repository filled...",
                UrlSlug = "aspnet-core-diagnnostic-scenarios",
                Published = true,
                PostedDate = new DateTime(2021, 9, 30, 10, 20, 0),
                ModifiedDate = null,
                ViewCount = 10,
                Author = authors[0],
                Category = categories[0],
                Tags = new List<Tag>()
                {
                    tags[1]
                }
            },
            new()
            {

                Title = "Design Patterns Information",
                ShortDescription = "Design pattern in software engineering is a general, reusable solution to a commonly occurring problem in software design.",
                Description = "Design patterns are used to solve these commonly occurring problems in the development phase so that we can minimize the problems after deployment. A design pattern suggests a specific implementation for the specific object-oriented programming problem. For example, if you want to ensure that only a single instance of a class exists, then you can use the Singleton design pattern which suggests the best way to create a class that can only have one object.",
                Meta = "Nothing to read...",
                UrlSlug = "design-patterns-information",
                Published = true,
                PostedDate = new DateTime(2021, 10, 2, 8, 33, 2),
                ModifiedDate = null,
                ViewCount = 2,
                Author = authors[1],
                Category = categories[4],
                Tags = new List<Tag>()
                {
                    tags[18]
                }
            },
            new()
            {
                Title = "Architecture Art",
                ShortDescription = "Architecture is the art and technique of designing and building, as distinguished from the skills associated with construction.",
                Description = "It is both the process and the product of sketching, conceiving, planning, designing, and constructing buildings or other structures. The term comes from Latin architectura; from Ancient Greek ἀρχιτέκτων (arkhitéktōn) 'architect'; from ἀρχι- (arkhi-) 'chief', and τέκτων (téktōn) 'creator'. Architectural works, in the material form of buildings, are often perceived as cultural symbols and as works of art. Historical civilizations are often identified with their surviving architectural achievements.",
                Meta = "Nothing to read...",
                UrlSlug = "architecture-art",
                Published = true,
                PostedDate = new DateTime(2021, 9, 28, 13, 48, 4),
                ModifiedDate = null,
                ViewCount = 5,
                Author = authors[3],
                Category = categories[1],
                Tags = new List<Tag>()
                {
                    tags[0]
                }
            },
            new()
            {
                Title = "Firebase Cloud Messaging",
                ShortDescription = "Firebase Cloud Messaging (FCM) is a cross-platform messaging solution that lets you reliably send messages at no cost.",
                Description = "Using FCM, you can notify a client app that new email or other data is available to sync. You can send notification messages to drive user re-engagement and retention. For use cases such as instant messaging, a message can transfer a payload of up to 4 KB to a client app.",
                Meta = "Nothing to read...",
                UrlSlug = "firebase-cloud-messaging",
                Published = true,
                PostedDate = new DateTime(2021, 10, 5, 14, 23, 6),
                ModifiedDate = null,
                ViewCount = 15,
                Author = authors[3],
                Category = categories[2],
                Tags = new List<Tag>()
                {
                    tags[0]
                }
            },
            new()
            {
                Title = "ReactJs Basic",
                ShortDescription = "ReactJS – Những điều bạn cần phải biết",
                Description = "ReactJS là một thư viện JavaScript mã nguồn mở được thiết kế bởi Facebook để tạo ra những ứng dụng web hấp dẫn, nhanh và hiệu quả với mã hóa tối thiểu. Mục đích cốt lõi của ReactJS không chỉ khiến cho trang web phải thật mượt mà còn phải nhanh, khả năng mở rộng cao và đơn giản.",
                Meta = "Nothing to read...",
                UrlSlug = "reactjc-basic",
                Published = true,
                PostedDate = new DateTime(2021, 10, 5, 14, 23, 6),
                ModifiedDate = null,
                ViewCount = 15,
                Author = authors[4],
                Category = categories[7],
                Tags = new List<Tag>()
                {
                    tags[13]
                }
            },
            new()
            {
                Title = "VueJs Introduction",
                ShortDescription = "Vue (pronounced /vjuː/, like view) is a JavaScript framework for building user interfaces. It builds on top of standard HTML, CSS, and JavaScript and provides a declarative and component-based programming model that helps you efficiently develop user interfaces, be they simple or complex.",
                Description = "Vue is a framework and ecosystem that covers most of the common features needed in frontend development. But the web is extremely diverse - the things we build on the web may vary drastically in form and scale. With that in mind, Vue is designed to be flexible and incrementally adoptable.",
                Meta = "Nothing to read...",
                UrlSlug = "vuejs-introduction",
                Published = true,
                PostedDate = new DateTime(2021, 10, 5, 14, 23, 6),
                ModifiedDate = null,
                ViewCount = 15,
                Author = authors[2],
                Category = categories[5],
                Tags = new List<Tag>()
                {
                    tags[9]
                }
            },
            new()
            {
                Title = "Introduction to Node.js",
                ShortDescription = "Getting started guide to Node.js, the server-side JavaScript runtime environment. Node.js is built on top of the Google Chrome V8 JavaScript engine, and it's mainly used to create web servers - but it's not limited to just that.",
                Description = "In Node.js the new ECMAScript standards can be used without problems, as you don't have to wait for all your users to update their browsers - you are in charge of deciding which ECMAScript version to use by changing the Node.js version, and you can also enable specific experimental features by running Node.js with flags.",
                Meta = "Nothing to read...",
                UrlSlug = "introduction-to-nodejs",
                Published = true,
                PostedDate = new DateTime(2021, 10, 5, 14, 23, 6),
                ModifiedDate = null,
                ViewCount = 15,
                Author = authors[0],
                Category = categories[9],
                Tags = new List<Tag>()
                {
                    tags[0]
                }
            },
            new()
            {
                Title = "Overview of ASP.NET Core",
                ShortDescription = "ASP.NET Core is a cross-platform, high-performance, open-source framework for building modern, cloud-enabled, Internet-connected apps.",
                Description = "ASP.NET Core is a cross-platform, high-performance, open-source framework for building modern, cloud-enabled, Internet-connected apps.",
                Meta = "Nothing to read...",
                UrlSlug = "overview-of-aspnet-core",
                Published = true,
                PostedDate = new DateTime(2021, 10, 5, 14, 23, 6),
                ModifiedDate = null,
                ViewCount = 15,
                Author = authors[1],
                Category = categories[3],
                Tags = new List<Tag>()
                {
                    tags[1]
                }
            },
            new()
            {
                Title = "React Router",
                ShortDescription = "Routes are perhaps the most important part of a React Router app.",
                Description = "Routes are perhaps the most important part of a React Router app. They couple URL segments to components, data loading and data mutations. Through route nesting, complex application layouts and data dependencies become simple and declarative.",
                Meta = "Nothing to read...",
                UrlSlug = "react-router",
                Published = true,
                PostedDate = new DateTime(2021, 10, 5, 14, 23, 6),
                ModifiedDate = null,
                ViewCount = 15,
                Author = authors[4],
                Category = categories[7],
                Tags = new List<Tag>()
                {
                    tags[13]
                }
            },
            new()
            {
                Title = "Introduction NPM",
                ShortDescription = "An introduction to the NPM package manager",
                Description = "A quick guide to npm, the powerful package manager key to the success of Node.js. In January 2017 over 350000 packages were reported being listed in the npm registry, making it the biggest single language code repository on Earth, and you can be sure there is a package for (almost!) everything.",
                Meta = "Nothing to read...",
                UrlSlug = "introduction-npm",
                Published = true,
                PostedDate = new DateTime(2021, 10, 5, 14, 23, 6),
                ModifiedDate = null,
                ViewCount = 15,
                Author = authors[4],
                Category = categories[8],
                Tags = new List<Tag>()
                {
                    tags[0]
                }
            }
        };

        _dbContext.Posts.AddRange(posts);
        _dbContext.SaveChanges();

        return posts;
    }
}
