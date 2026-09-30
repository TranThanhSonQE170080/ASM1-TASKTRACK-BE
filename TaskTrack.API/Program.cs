using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.Data;
using TaskTrack.Repo.Repositories;
using TaskTrack.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// ---- EF Core + PostgreSQL (Database-First) ----
var connectionString = builder.Configuration["DATABASE_URL"]
    ?? builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

// Support Render-style PostgreSQL URLs (postgres:// or postgresql://user:pass@host/db)
if (connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase)
    || connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
    connectionString = ConvertPostgresUrl(connectionString);

builder.Services.AddDbContext<TaskManagementContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ---- Services ----
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<ISummaryService, SummaryService>();

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// ---- Swagger (dev only) ----
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ---- CORS: allow Vercel frontend ----
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy => policy
        .WithOrigins(
            "http://localhost:3000",
            "http://localhost:3001",
            "http://127.0.0.1:3000",
            "http://127.0.0.1:3001",
            builder.Configuration["FrontendUrl"] ?? "https://your-frontend.vercel.app")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

var renderPort = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(renderPort) && string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPNETCORE_URLS")))
    app.Urls.Add($"http://0.0.0.0:{renderPort}");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Frontend");
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
app.Run();

// postgres://user:pass@host:port/db -> Host=..;Username=..;..
static string ConvertPostgresUrl(string url)
{
    var uri = new Uri(url);
    var userInfo = uri.UserInfo.Split(':', 2);
    var db = uri.AbsolutePath.TrimStart('/');
    return $"Host={uri.Host};Port={uri.Port};Database={db};Username={Uri.UnescapeDataString(userInfo[0])};Password={Uri.UnescapeDataString(userInfo[1])}";
}
