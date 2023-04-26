namespace TatBlog.Services.Blogs;

public interface IDashboardRepository
{
    Task<int> GetTotalOfPostsAsync();

    Task<int> GetTotalOfUnpublishedPostsAsync();

    Task<int> GetTotalOfCategoriesAsync();

    Task<int> GetTotalOfAuthorsAsync();

    Task<int> GetTotalOfWaitingCommentAsync();

    Task<int> GetTotalOfSubscriberAsync();

    Task<int> GetTotalOfNewestSubscriberInDayAsync();
}