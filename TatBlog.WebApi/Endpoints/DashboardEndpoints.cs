using TatBlog.Services.Blogs;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Endpoints;

public static class DashboardEndpoints
{
    public static WebApplication MapDashboardEndpoints(this WebApplication app)
    {
        var routeGroupBuilder = app.MapGroup("/api/dashboard");

        routeGroupBuilder.MapGet("/total/posts", GetTotalOfPosts)
                         .WithName("GetTotalOfPosts")
                         .Produces<ApiResponse<int>>();

        routeGroupBuilder.MapGet("/total/posts/unpublished", GetTotalOfUnpublishedPosts)
                         .WithName("GetTotalOfUnpublishedPosts")
                         .Produces<ApiResponse<int>>();

        routeGroupBuilder.MapGet("/total/categories", GetTotalOfCategories)
                         .WithName("GetTotalOfCategories")
                         .Produces<ApiResponse<int>>();

        routeGroupBuilder.MapGet("/total/authors", GetTotalOfAuthors)
                         .WithName("GetTotalOfAuthors")
                         .Produces<ApiResponse<int>>();

        routeGroupBuilder.MapGet("/total/comments/waiting", GetTotalOfWaitingComment)
                         .WithName("GetTotalOfWaitingComment")
                         .Produces<ApiResponse<int>>();

        routeGroupBuilder.MapGet("/total/subscribers", GetTotalOfSubscriber)
                         .WithName("GetTotalOfSubscriber")
                         .Produces<ApiResponse<int>>();

        routeGroupBuilder.MapGet("/total/subscribers/newest", GetTotalOfNewestSubscriberInDay)
                         .WithName("GetTotalOfNewestSubscriberInDay")
                         .Produces<ApiResponse<int>>();

        return app;
    }

    private static async Task<IResult> GetTotalOfPosts(IDashboardRepository dashboardRepository)
    {
        var totalPost = await dashboardRepository.GetTotalOfPostsAsync();

        return Results.Ok(ApiResponse.Success(totalPost));
    }

    private static async Task<IResult> GetTotalOfUnpublishedPosts(IDashboardRepository dashboardRepository)
    {
        var totalPost = await dashboardRepository.GetTotalOfUnpublishedPostsAsync();

        return Results.Ok(ApiResponse.Success(totalPost));
    }

    private static async Task<IResult> GetTotalOfCategories(IDashboardRepository dashboardRepository)
    {
        var totalCategory = await dashboardRepository.GetTotalOfCategoriesAsync();

        return Results.Ok(ApiResponse.Success(totalCategory));
    }

    private static async Task<IResult> GetTotalOfAuthors(IDashboardRepository dashboardRepository)
    {
        var totalAuthor = await dashboardRepository.GetTotalOfAuthorsAsync();

        return Results.Ok(ApiResponse.Success(totalAuthor));
    }

    private static async Task<IResult> GetTotalOfWaitingComment(IDashboardRepository dashboardRepository)
    {
        var total = await dashboardRepository.GetTotalOfWaitingCommentAsync();

        return Results.Ok(ApiResponse.Success(total));
    }

    private static async Task<IResult> GetTotalOfSubscriber(IDashboardRepository dashboardRepository)
    {
        var totalSubscriber = await dashboardRepository.GetTotalOfSubscriberAsync();

        return Results.Ok(ApiResponse.Success(totalSubscriber));
    }

    private static async Task<IResult> GetTotalOfNewestSubscriberInDay(IDashboardRepository dashboardRepository)
    {
        var total = await dashboardRepository.GetTotalOfNewestSubscriberInDayAsync();

        return Results.Ok(ApiResponse.Success(total));
    }
}
