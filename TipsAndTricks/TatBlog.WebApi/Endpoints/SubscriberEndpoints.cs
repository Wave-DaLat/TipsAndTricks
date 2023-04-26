using FluentValidation;
using MapsterMapper;
using System.Net;
using TatBlog.Core.Collections;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.WebApi.Filters;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Endpoints;

public static class SubscriberEndpoints
{
    public static WebApplication MapSubscriberEndpoints(this WebApplication app)
    {
        var routeGroupBuilder = app.MapGroup("/api/subscribers");

        routeGroupBuilder.MapGet("/", GetSubscribers)
                         .WithName("GetSubscribers")
                         .Produces<ApiResponse<PaginationResult<Subscriber>>>();

        routeGroupBuilder.MapGet("/{id:int}", GetSubscriberDetails)
                         .WithName("GetSubscriberById")
                         .Produces<ApiResponse<Subscriber>>();

        routeGroupBuilder.MapGet("/email/{email}", GetSubscriberByEmailDetails)
                         .WithName("GetSubscriberByEmail")
                         .Produces<ApiResponse<Subscriber>>();

        routeGroupBuilder.MapPost("/", Subscribe)
                         .WithName("NewSubscriber")
                         .AddEndpointFilter<ValidatorFilter<SubscriberEditModel>>()
                         .Produces(401)
                         .Produces<ApiResponse<Subscriber>>();

        routeGroupBuilder.MapPost("/unsub/{email}", Unsubscribe)
                         .WithName("NewUnsubscriber")
                         .AddEndpointFilter<ValidatorFilter<SubscriberEditModel>>()
                         .Produces(401)
                         .Produces<ApiResponse<string>>();

        routeGroupBuilder.MapDelete("/{id:int}", DeleteSubscriber)
                         .WithName("DeleteSubscriber")
                         .Produces(401)
                         .Produces<ApiResponse<string>>();

        routeGroupBuilder.MapPost("/{id:int}", BlockSubscriber)
                         .WithName("BlockSubscriber")
                         .AddEndpointFilter<ValidatorFilter<SubscriberEditModel>>()
                         .Produces(401)
                         .Produces<ApiResponse<string>>();

        return app;
    }

    private static async Task<IResult> GetSubscribers([AsParameters] SubscriberFilterModel model, ISubscriberRepository subscriberRepository, IMapper mapper)
    {
        var subscriberQuery = mapper.Map<SubscriberQuery>(model);
        var subscriberList = await subscriberRepository.GetSubscriberByQueryAsync(subscriberQuery, model);

        var paginationResult = new PaginationResult<Subscriber>(subscriberList);

        return Results.Ok(ApiResponse.Success(paginationResult));
    }

    private static async Task<IResult> GetSubscriberDetails(int id, ISubscriberRepository subscriberRepository)
    {
        var subscriber = await subscriberRepository.GetCachedSubscriberByIdAsync(id);

        return subscriber == null ? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Không tìm thấy người đăng kí có mã số {id}")) : Results.Ok(ApiResponse.Success(subscriber));
    }

    private static async Task<IResult> GetSubscriberByEmailDetails(string email, ISubscriberRepository subscriberRepository)
    {
        var subscriber = await subscriberRepository.GetCachedSubscriberByEmailAsync(email);

        return subscriber == null ? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Không tìm thấy người đăng kí có email {email}")) : Results.Ok(ApiResponse.Success(subscriber));
    }

    private static async Task<IResult> Subscribe(SubscriberEditModel model, ISubscriberRepository subscriberRepository, IMapper mapper)
    {
        var subscriber = mapper.Map<Subscriber>(model);

        return await subscriberRepository.SubscribeAsync(subscriber.SubscribeEmail) ? Results.Ok(ApiResponse.Success("Subscriber is registered", HttpStatusCode.NoContent)) : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, "Could not found subscriber"));
    }

    private static async Task<IResult> Unsubscribe(string email, ISubscriberRepository subscriberRepository)
    {
        return await subscriberRepository.UnsubscribeAsync(email, "Không có nhu cầu nữa", true) ? Results.Ok(ApiResponse.Success("Subscriber is unregistered", HttpStatusCode.NoContent)) : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, "Could not found subscriber"));
    }

    private static async Task<IResult> DeleteSubscriber(int id, ISubscriberRepository subscriberRepository)
    {
        return await subscriberRepository.DeleteSubscriberAsync(Convert.ToInt32(id)) ? Results.Ok(ApiResponse.Success("Subscriber is deleted", HttpStatusCode.NoContent)) : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Could not find subscriber with id = {id}"));
    }

    private static async Task<IResult> BlockSubscriber(int id, SubscriberEditModel model, ISubscriberRepository subscriberRepository, IMapper mapper)
    {
        var subscriber = mapper.Map<Subscriber>(model);
        subscriber.Id = id;

        return await subscriberRepository.BlockSubscriberAsync(id, subscriber.CancelReason, subscriber.AdminNotes) ? Results.Ok(ApiResponse.Success("Subscriber is blocked", HttpStatusCode.NoContent)) : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Could not find subscriber with id = {id}"));
    }
}
