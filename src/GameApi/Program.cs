using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure settings
builder.Services.Configure<GameApi.Settings>(builder.Configuration.GetSection("GameApi"));

// Add storage and services
builder.Services.AddSingleton<GameApi.Storage.ITableStore, GameApi.Storage.AzureTableStore>();
builder.Services.AddSingleton<GameApi.Services.KeyService>();
builder.Services.AddSingleton<GameApi.Services.GameService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

namespace GameApi {}
