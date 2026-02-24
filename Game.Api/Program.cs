using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Azure.Data.Tables;
using Microsoft.Extensions.Caching.Memory;
using Game.Api.Storage.Repositories;
using Game.Api.Services;
using Game.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// register Azure Table client
var conn = config.GetValue<string>("AZURE_TABLES_CONNECTION_STRING");
var svc = new TableServiceClient(conn);
builder.Services.AddSingleton(svc);

// repositories
builder.Services.AddSingleton<PlayerRepository>();
builder.Services.AddSingleton<ShipRepository>();
builder.Services.AddSingleton<MarketRepository>();
builder.Services.AddSingleton<SkillRepository>();
builder.Services.AddSingleton<InventoryRepository>();
builder.Services.AddSingleton<SystemEventRepository>();

// services
builder.Services.Configure<MarketTickOptions>(config.GetSection("MarketTick"));
builder.Services.AddSingleton<GameTimeService>();
builder.Services.AddHostedService<MarketTickService>();
builder.Services.AddHostedService<NewsFeedService>();

builder.Services.Configure<NewsFeedOptions>(config.GetSection("NewsFeed"));

builder.Services.AddMemoryCache();

builder.Services.AddControllers();
var app = builder.Build();

// use rate limit middleware
app.UseMiddleware<RateLimitMiddleware>();

app.MapControllers();
app.Run();
