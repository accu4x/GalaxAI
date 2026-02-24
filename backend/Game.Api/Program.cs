using Game.Auth.Services;
using Game.Storage.Adapters;

var builder = WebApplication.CreateBuilder(args);

// Load user-secrets in development so local secrets.json values are available
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>(optional: true);
}

// Configuration - prefer explicit environment variable, then config (user-secrets or appsettings).
var tablesConnection = Environment.GetEnvironmentVariable("AZURE_TABLES_CONNECTION_STRING")
    ?? builder.Configuration["AZURE_TABLES_CONNECTION_STRING"]
    ?? builder.Configuration["AzureTables:ConnectionString"];
if (string.IsNullOrWhiteSpace(tablesConnection))
{
    Console.WriteLine("Warning: AZURE_TABLES_CONNECTION_STRING not set. Some features will be disabled.");
}

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register storage and auth services
builder.Services.AddSingleton(new TableStorageAdapter(tablesConnection ?? string.Empty));
builder.Services.AddSingleton(new ApiKeyService(tablesConnection ?? string.Empty));

// Register NPC skill (local data path inside the repo)
var npcDataPath = Path.Combine(builder.Environment.ContentRootPath, "Game.NpcSkill", "Data", "NPCs");
builder.Services.AddSingleton(new Game.NpcSkill.NpcService(npcDataPath));

// Register DialogRunner for sessions (used by Telegram bot)
var dialogDataPath = Path.Combine(builder.Environment.ContentRootPath, "Game.NpcSkill", "Data", "NPCs");
builder.Services.AddSingleton(new Game.NpcSkill.Dialog.DialogRunner(dialogDataPath));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
