using Elastic.CommonSchema;
using FiapCloudGamesUsers.Api.Authorize;
using FiapCloudGamesUsers.Api.Endpoints;
using FiapCloudGamesUsers.Api.Extensions;
using FiapCloudGamesUsers.Application.Middlewares;
using FiapCloudGamesUsers.Application.Services;
using FiapCloudGamesUsers.Application.Services.Interfaces;
using FiapCloudGamesUsers.Application.Validators;
using FiapCloudGamesUsers.Domain.Abstractions;
using FiapCloudGamesUsers.Domain.Events;
using FiapCloudGamesUsers.Domain.Repositories;
using FiapCloudGamesUsers.Infra.Data.Context;
using FiapCloudGamesUsers.Infra.Data.Messaging;
using FiapCloudGamesUsers.Infra.Data.Repositories;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.AddElasticConfiguration();
builder.AddMassTransitConfiguration();

var serverVersion = new MySqlServerVersion(new Version(8, 0));
builder.Services.AddDbContext<ContextDb>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("MySQL"), serverVersion);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddAuthentication(opt => {
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        ValidAudience = builder.Configuration["Authentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authentication:Key"])),
        RoleClaimType = ClaimTypes.Role
    };
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SameUserOrAdmin", policy =>
        policy.Requirements.Add(new SameUserRequirement()));
});
builder.Services.AddScoped<IAuthorizationHandler, SameUserHandler>();
builder.Services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();
builder.Services.AddValidatorsFromAssemblyContaining<UserRequestDtoValidator>();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(UserCreatedEventHandler).Assembly);
});

var app = builder.Build();

if (args.Contains("migrate"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ContextDb>();
    db.Database.Migrate();
    return;
}

app.UseMiddleware<ExceptionHandlerMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();

app.UseHsts();

app.MapUserEndpoints();
app.MapAuthEndpoints();

app.Run();
