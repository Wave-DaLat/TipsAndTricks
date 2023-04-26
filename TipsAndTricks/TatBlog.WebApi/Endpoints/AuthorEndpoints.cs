using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TatBlog.Core.Collections;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
using TatBlog.WebApi.Filters;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Endpoints;

public static class AuthorEndpoints
{
    public static WebApplication MapAuthorEndpoints(this WebApplication app)
    {
        var routeGroupBuilder = app.MapGroup("/api/authors");

        routeGroupBuilder.MapGet("/", GetAuthors)
                         .WithName("GetAuthors")
                         .Produces<ApiResponse<PaginationResult<AuthorItem>>>();

        routeGroupBuilder.MapGet("/{id:int}", GetAuthorDetails)
                         .WithName("GetAuthorById")
                         .Produces<ApiResponse<AuthorItem>>();

        routeGroupBuilder.MapGet("/{slug::regex(^[a-z0-9_-]+$)}/posts", GetPostByAuthorSlug)
                         .WithName("GetPostByAuthorSlug")
                         .Produces<ApiResponse<PaginationResult<PostDto>>>();

        routeGroupBuilder.MapPost("/", AddAuthor)
                         .WithName("AddNewAuthor")
                         .AddEndpointFilter<ValidatorFilter<AuthorEditModel>>()
                         .Produces(401)
                         .Produces<ApiResponse<AuthorItem>>(); ;

        routeGroupBuilder.MapPut("/{id:int}", UpdateAuthor)
                         .WithName("UpdateAuthor")
                         .AddEndpointFilter<ValidatorFilter<AuthorEditModel>>()
                         .Produces(401)
                         .Produces<ApiResponse<string>>();

        routeGroupBuilder.MapDelete("/{id:int}", DeleteAuthor)
                         .WithName("DeleteAuthor")
                         .Produces(401)
                         .Produces<ApiResponse<string>>();

        routeGroupBuilder.MapPost("/{id:int}/picture", SetAuthorPicture)
                         .WithName("SetAuthorPicture")
                         .Accepts<IFormFile>("multipart/formdata")
                         .Produces(401)
                         .Produces<ApiResponse<string>>();

        routeGroupBuilder.MapGet("/best/{limit:int}", GetAuthorHasBestPosts)
            .WithName("GetAuthorHasBestPosts")
            .Produces<ApiResponse<IList<Author>>>();

        return app;
    }

    private static async Task<IResult> GetAuthors([AsParameters] AuthorFilterModel model, IAuthorRepository authorRepository, IMapper mapper)
    {
        var authorQuery = mapper.Map<AuthorQuery>(model);
        var authorList = await authorRepository.GetAuthorByQueryAsync(authorQuery, model, author => author.ProjectToType<AuthorItem>());

        var paginationResult = new PaginationResult<AuthorItem>(authorList);

        return Results.Ok(ApiResponse.Success(paginationResult));
    }

    private static async Task<IResult> GetAuthorDetails(int id, IAuthorRepository authorRepository, IMapper mapper)
    {
        var author = await authorRepository.GetCachedAuthorByIdAsync(id);

        return author == null ? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Không tìm thấy tác giả có mã số {id}")) : Results.Ok(ApiResponse.Success(mapper.Map<AuthorItem>(author)));
    }

    private static async Task<IResult> GetPostByAuthorId(int id, [AsParameters] PagingModel pagingModel, IBlogRepository blogRepository)
    {
        var postQuery = new PostQuery
        {
            AuthorId = id,
            PublishedOnly = true
        };

        var postsList = await blogRepository.GetPostByQueryAsync(postQuery, pagingModel, posts => posts.ProjectToType<PostDto>());

        var paginationResult = new PaginationResult<PostDto>(postsList);

        return Results.Ok(ApiResponse.Success(paginationResult));
    }

    private static async Task<IResult> GetPostByAuthorSlug([FromRoute] string slug, [AsParameters] PagingModel pagingModel, IBlogRepository blogRepository)
    {
        var postQuery = new PostQuery
        {
            AuthorSlug = slug,
            PublishedOnly = true
        };

        var postsList = await blogRepository.GetPostByQueryAsync(postQuery, pagingModel, posts => posts.ProjectToType<PostDto>());

        var paginationResult = new PaginationResult<PostDto>(postsList);

        return Results.Ok(ApiResponse.Success(paginationResult));
    }

    private static async Task<IResult> AddAuthor(AuthorEditModel model, IAuthorRepository authorRepository, IMapper mapper)
    {
        if (await authorRepository.CheckAuthorSlugExisted(0, model.UrlSlug))
        {
            return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict, $"Slug '{model.UrlSlug}' đã được sử dụng"));
        }

        var author = mapper.Map<Author>(model);
        await authorRepository.AddOrUpdateAuthorAsync(author);

        return Results.Ok(ApiResponse.Success(mapper.Map<AuthorItem>(author), HttpStatusCode.Created));
    }

    private static async Task<IResult> UpdateAuthor(int id, AuthorEditModel model, IAuthorRepository authorRepository, IMapper mapper)
    {
        if (await authorRepository.CheckAuthorSlugExisted(id, model.UrlSlug))
        {
            return Results.Ok(ApiResponse.Fail(HttpStatusCode.BadRequest, $"Slug '{model.UrlSlug}' đã được sử dụng"));
        }

        var author = mapper.Map<Author>(model);
        author.Id = id;

        return await authorRepository.AddOrUpdateAuthorAsync(author) ? Results.Ok(ApiResponse.Success("Author is updated", HttpStatusCode.NoContent)) : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, "Could not found author"));
    }

    private static async Task<IResult> DeleteAuthor(int id, IAuthorRepository authorRepository)
    {
        return await authorRepository.DeleteAuthorByIdAsync(id) ? Results.Ok(ApiResponse.Success("Author is deleted", HttpStatusCode.NoContent)) : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Could not find author with id = {id}"));
    }

    private static async Task<IResult> SetAuthorPicture(int id, IFormFile imageFile, IAuthorRepository authorRepository, IMediaManager mediaManager)
    {
        var imageUrl = await mediaManager.SaveFileAsync(imageFile.OpenReadStream(), imageFile.FileName, imageFile.ContentType);

        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return Results.Ok(ApiResponse.Fail(HttpStatusCode.BadRequest, "Không lưu được tập tin"));
        }

        await mediaManager.DeleteFileAsync(imageUrl);
        await authorRepository.SetImageUrlAsync(id, imageUrl);

        return Results.Ok(ApiResponse.Success(imageUrl));
    }

    private static async Task<IResult> GetAuthorHasBestPosts(
        int limit,
        IAuthorRepository authorRepository)
    {
        var authors = await authorRepository.Find_N_PostsByAuthorAsync(limit);

        return Results.Ok(ApiResponse.Success(authors));
    }
}
