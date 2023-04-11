using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TatBlog.Data.Contexts;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
using TatBlog.Services.Timing;

namespace TatBlog.WebApi.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplicationBuilder ConfigureServices(
        this WebApplicationBuilder builder)
    {
        builder.Services.AddOptions();

        builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

        builder.Services.AddMemoryCache();

        builder.Services.AddDbContext<BlogDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration
                    .GetConnectionString("DefaultConnection")));

        builder.Services.AddTransient<SendMailService>();
        builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
        builder.Services.AddScoped<IBlogRepository, BlogRepository>();
        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
        builder.Services.AddScoped<ICommentRepository, CommentRepository>();
        builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
        builder.Services.AddScoped<IMediaManager, LocalFileSystemMediaManager>();
        builder.Services.AddScoped<ITagRepository, TagRepository>();
        builder.Services.AddScoped<ITimeProvider, LocalTimeProvider>();
        builder.Services.AddScoped<ISubscriberRepository, SubscriberRepository>();

        return builder;
    }

    public static WebApplicationBuilder ConfigureCors(
        this WebApplicationBuilder builder )
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("TatBlogApp", policyBuilder =>
                policyBuilder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());
        });

        return builder;
    }

    public static WebApplicationBuilder ConfigureSwaggerOpenApi(
        this WebApplicationBuilder builder )
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddControllersWithViews()
                            .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

        return builder;
    }

    public static WebApplication SetupRequestPipeline(
        this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseStaticFiles();
        app.UseHttpsRedirection();

        app.UseCors("TatBlogApp");

        return app;
    }
}
