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

public static class TagEndpoints
{
    public static WebApplication MapTagEndpoints(this WebApplication app)
    {
        var routeGroupBuilder = app.MapGroup("/api/tags");

        routeGroupBuilder.MapGet("/", GetTags)
                 .WithName("GetTags")
                 .Produces<ApiResponse<PaginationResult<TagItem>>>();

        routeGroupBuilder.MapGet("/all", GetAllTags)
                 .WithName("GetAllTags")
                 .Produces<ApiResponse<TagItem>>();

        routeGroupBuilder.MapGet("/{id:int}", GetTagDetails)
                 .WithName("GetTagById")
                 .Produces<ApiResponse<TagItem>>();

        routeGroupBuilder.MapGet("/{slug::regex(^[a-z0-9_-]+$)}/posts", GetPostByTagSlug)
                 .WithName("GetPostByTagSlug")
                 .Produces<ApiResponse<PaginationResult<PostDto>>>();

        routeGroupBuilder.MapPost("/", AddTag)
                 .WithName("AddNewTag")
                 .AddEndpointFilter<ValidatorFilter<TagEditModel>>()
                 .Produces(401)
                 .Produces<ApiResponse<TagItem>>();

        routeGroupBuilder.MapPut("/{id:int}", UpdateTag)
                 .WithName("UpdateTag")
                 .AddEndpointFilter<ValidatorFilter<TagEditModel>>()
                 .Produces(401)
                 .Produces<ApiResponse<string>>();

        routeGroupBuilder.MapDelete("/{id:int}", DeleteTag)
                 .WithName("DeleteTag")
                 .Produces(401)
                 .Produces<ApiResponse<string>>();

        return app;
    }

    private static async Task<IResult> GetTags([AsParameters] TagFilterModel model, ITagRepository tagRepository, IMapper mapper)
    {
        var tagQuery = mapper.Map<TagQuery>(model);
        var tagList = await tagRepository.GetTagByQueryAsync(tagQuery, model, tag => tag.ProjectToType<TagItem>());

        var paginationResult = new PaginationResult<TagItem>(tagList);

        return Results.Ok(ApiResponse.Success(paginationResult));
    }

    private static async Task<IResult> GetAllTags(ITagRepository tagRepository, IMapper mapper)
    {
        var tagList = await tagRepository.GetTagListAsync();

        var tagDto = tagList.Select(t => mapper.Map<TagItem>(t));

        return Results.Ok(ApiResponse.Success(tagDto));
    }

    private static async Task<IResult> GetTagDetails(int id, ITagRepository tagRepository, IMapper mapper)
    {
        var tag = await tagRepository.GetCachedTagByIdAsync(id);

        return tag == null ? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Không tìm thấy thẻ có mã số {id}")) : Results.Ok(ApiResponse.Success(mapper.Map<AuthorItem>(tag)));
    }

    private static async Task<IResult> GetPostByTagSlug([FromRoute] string slug, [AsParameters] PagingModel pagingModel, IBlogRepository blogRepository)
    {
        var postQuery = new PostQuery
        {
            TagSlug = slug,
            PublishedOnly = true
        };

        var postsList = await blogRepository.GetPostByQueryAsync(postQuery, pagingModel, posts => posts.ProjectToType<PostDto>());

        var paginationResult = new PaginationResult<PostDto>(postsList);

        return Results.Ok(ApiResponse.Success(paginationResult));
    }

    private static async Task<IResult> AddTag(TagEditModel model, ITagRepository tagRepository, IMapper mapper)
    {
        if (await tagRepository.CheckTagSlugExisted(0, model.UrlSlug))
        {
            return Results.Conflict($"Slug '{model.UrlSlug}' đã được sử dụng");
        }

        var tag = mapper.Map<Tag>(model);
        await tagRepository.AddOrUpdateTagAsync(tag);

        return Results.Ok(ApiResponse.Success(mapper.Map<AuthorItem>(tag), HttpStatusCode.Created));
    }

    private static async Task<IResult> UpdateTag(int id, TagEditModel model, ITagRepository tagRepository, IMapper mapper)
    {
        if (await tagRepository.CheckTagSlugExisted(id, model.UrlSlug))
        {
            return Results.Ok(ApiResponse.Fail(HttpStatusCode.BadRequest, $"Slug '{model.UrlSlug}' đã được sử dụng"));
        }

        var tag = mapper.Map<Tag>(model);
        tag.Id = id;

        return await tagRepository.AddOrUpdateTagAsync(tag) ? Results.Ok(ApiResponse.Success("Tag is updated", HttpStatusCode.NoContent)) : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, "Could not found tag"));
    }

    private static async Task<IResult> DeleteTag(int id, ITagRepository tagRepository)
    {
        return await tagRepository.DeleteTagByIdAsync(id) ? Results.Ok(ApiResponse.Success("Tag is deleted", HttpStatusCode.NoContent)) : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Could not find tag with id = {id}"));
    }
}
