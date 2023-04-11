using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TatBlog.Core.Collections;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.WebApi.Filters;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Endpoints;

public static class CategoryEndpoints
{
    public static WebApplication MapCategoryEndpoints(this WebApplication app)
    {
        var routeGroupBuilder = app.MapGroup("/api/categories");

        routeGroupBuilder.MapGet("/", GetCategories)
                         .WithName("GetCategories")
                         .Produces<ApiResponse<PaginationResult<CategoryItem>>>();

        routeGroupBuilder.MapGet("/{id:int}", GetCategoryDetails)
                         .WithName("GetCategoryById")
                         .Produces<ApiResponse<CategoryItem>>();

        routeGroupBuilder.MapGet("/{slug::regex(^[a-z0-9_-]+$)}/posts", GetPostByCategorySlug)
                         .WithName("GetPostByCategorySlug")
                         .Produces<ApiResponse<PaginationResult<PostDto>>>();

        routeGroupBuilder.MapPost("/", AddCategory)
                         .WithName("AddNewCategory")
                         .AddEndpointFilter<ValidatorFilter<CategoryEditModel>>()
                         .Produces<ApiResponse<CategoryItem>>();

        routeGroupBuilder.MapPut("/{id:int}", UpdateCategory)
                         .WithName("UpdateCategory")
                         .AddEndpointFilter<ValidatorFilter<CategoryEditModel>>()
                         .Produces<ApiResponse<string>>();

        routeGroupBuilder.MapDelete("/{id:int}", DeleteCategory)
                         .WithName("DeleteCategory")
                         .Produces<ApiResponse<string>>();

        return app;
    }

    private static async Task<IResult> GetCategories([AsParameters] CategoryFilterModel model, ICategoryRepository categoryRepository, IMapper mapper)
    {
        var categoryQuery = mapper.Map<CategoryQuery>(model);
        var categoryList = await categoryRepository.GetCategoryByQueryAsync(categoryQuery, model, category => category.ProjectToType<CategoryItem>());

        var paginationResult = new PaginationResult<CategoryItem>(categoryList);

        return Results.Ok(ApiResponse.Success(paginationResult));
    }

    private static async Task<IResult> GetCategoryDetails(int id, ICategoryRepository categoryRepository, IMapper mapper)
    {
        var category = await categoryRepository.GetCachedCategoryByIdAsync(id);

        return category == null ? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Không tìm thấy chuyên mục có mã số {id}")) : Results.Ok(ApiResponse.Success(mapper.Map<CategoryItem>(category)));
    }

    private static async Task<IResult> GetPostByCategoryId(int id, [AsParameters] PagingModel pagingModel, IBlogRepository blogRepository)
    {
        var postQuery = new PostQuery
        {
            CategoryId = id,
            PublishedOnly = true
        };

        var postsList = await blogRepository.GetPostByQueryAsync(postQuery, pagingModel, posts => posts.ProjectToType<PostDto>());

        var paginationResult = new PaginationResult<PostDto>(postsList);

        return Results.Ok(ApiResponse.Success(paginationResult));
    }

    private static async Task<IResult> GetPostByCategorySlug([FromRoute] string slug, [AsParameters] PagingModel pagingModel, IBlogRepository blogRepository)
    {
        var postQuery = new PostQuery
        {
            CategorySlug = slug,
        };

        var postsList = await blogRepository.GetPostByQueryAsync(postQuery, pagingModel, posts => posts.ProjectToType<PostDto>());

        var paginationResult = new PaginationResult<PostDto>(postsList);

        return Results.Ok(ApiResponse.Success(paginationResult));
    }

    private static async Task<IResult> AddCategory(CategoryEditModel model, ICategoryRepository categoryRepository, IMapper mapper)
    {
        if (await categoryRepository.CheckCategorySlugExisted(0, model.UrlSlug))
        {
            return Results.Conflict($"Slug '{model.UrlSlug}' đã được sử dụng");
        }

        var category = mapper.Map<Category>(model);
        await categoryRepository.AddOrUpdateCategoryAsync(category);

        return Results.Ok(ApiResponse.Success(mapper.Map<CategoryItem>(category), HttpStatusCode.Created));
    }

    private static async Task<IResult> UpdateCategory(int id, CategoryEditModel model, ICategoryRepository categoryRepository, IMapper mapper)
    {
        if (await categoryRepository.CheckCategorySlugExisted(id, model.UrlSlug))
        {
            return Results.Ok(ApiResponse.Fail(HttpStatusCode.BadRequest, $"Slug '{model.UrlSlug}' đã được sử dụng"));
        }

        var category = mapper.Map<Category>(model);
        category.Id = id;

        return await categoryRepository.AddOrUpdateCategoryAsync(category) ? Results.Ok(ApiResponse.Success("Category is updated", HttpStatusCode.NoContent)) : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, "Could not found category"));
    }

    private static async Task<IResult> DeleteCategory(int id, ICategoryRepository categoryRepository)
    {
        return await categoryRepository.DeleteCategoryByIdAsync(id) ? Results.Ok(ApiResponse.Success("Category is deleted", HttpStatusCode.NoContent)) : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Could not find category with id = {id}"));
    }
}
