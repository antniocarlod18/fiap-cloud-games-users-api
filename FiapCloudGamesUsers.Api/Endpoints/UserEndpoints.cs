
using FiapCloudGamesUsers.Application.Dtos;
using FiapCloudGamesUsers.Application.Services.Interfaces;
using FluentValidation;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace FiapCloudGamesUsers.Api.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/users", AddAsync)
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        endpoints.MapGet("/users/{userId}", Getsync)
            .RequireAuthorization("SameUserOrAdmin"); 

        endpoints.MapPut("/users/unlock", UnlockAsync)
            .RequireAuthorization(policy => policy.RequireRole("LockUser"));

        endpoints.MapPut("/users/{userId}/make-admin", MakeAdminAsync)
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        endpoints.MapPut("/users/{userId}/revoke-admin", RevokeAdminAsync)
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        endpoints.MapGet("/users", GetAllAsync)
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        endpoints.MapDelete("/users/{userId}", DeleteAsync)
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        endpoints.MapPut("/users/{userId}", UpdateAsync)
            .RequireAuthorization("SameUserOrAdmin");

        return endpoints;
    }

    public static async Task<IResult> AddAsync(UserRequestDto dto, IUserService service, IValidator<UserRequestDto> validator)
    {
        var validationResult = await validator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var created = await service.AddAsync(dto);
        return Results.Created("/users", created);
    }

    public static async Task<IResult> Getsync(Guid userId, IUserService service)
    {
        return Results.Ok(await service.GetAsync(userId));
    }

    public static async Task<IResult> UnlockAsync(UserUnlockRequestDto userUnlockRequestDto, HttpContext context, IUserService service, IValidator<UserUnlockRequestDto> validator)
    {
        var validationResult = await validator.ValidateAsync(userUnlockRequestDto);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        Guid.TryParse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId);
        return Results.Ok(await service.UnlockAsync(userId, userUnlockRequestDto));
    }

    public static async Task<IResult> MakeAdminAsync(Guid userId, IUserService service)
    {
        return Results.Ok(await service.MakeAdminAsync(userId));
    }

    public static async Task<IResult> RevokeAdminAsync(Guid userId, IUserService service)
    {
        return Results.Ok(await service.RevokeAdminAsync(userId));
    }

    public static async Task<IResult> GetAllAsync( IUserService service)
    {
        return Results.Ok(await service.GetAllAsync());
    }

    public static async Task<IResult> DeleteAsync(Guid userId, IUserService service)
    {
        await service.DeleteAsync(userId);
        return Results.Ok();
    }

    public static async Task<IResult> UpdateAsync(Guid userId, UserUpdateRequestDto dto, IUserService service, IValidator<UserUpdateRequestDto> validator)
    {
        var validationResult = await validator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        return Results.Ok(await service.UpdateAsync(userId, dto));
    }
}