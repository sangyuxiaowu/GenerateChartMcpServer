using GenerateChartMcpServer;
using Sang.AspNetCore.SignAuthorization;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

var allowedOrigins = config.GetSection("AllowedOrigins").Get<string[]>()?
    .Where(origin => !string.IsNullOrWhiteSpace(origin))
    .ToHashSet(StringComparer.OrdinalIgnoreCase)
    ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);

// Configure SignAuthorization
var apiKey = config["ApiKey"] ?? "";
ChartTools.SignOptions.sToken = apiKey;
ChartTools.SignOptions.Expire = int.TryParse(config["SignExpireSeconds"], out var exp) ? exp : 3600;
ChartTools.ImageBaseUrl = (config["ImageBaseUrl"] ?? "http://localhost:52345/images").TrimEnd('/');
ChartTools.ImageStoragePath = config["ImageStoragePath"] ?? "images";
ChartTools.ContentRootPath = builder.Environment.ContentRootPath;
ChartTools.ConfiguredFontName = config["FontName"];

builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithTools<ChartTools>();

var app = builder.Build();

// Serve images via endpoint with SignAuthorization
var imageDir = Path.Combine(app.Environment.ContentRootPath, ChartTools.ImageStoragePath);
Directory.CreateDirectory(imageDir);

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapGet("/images/{filename}", (string filename) =>
{
    var filePath = Path.Combine(imageDir, filename);
    if (!File.Exists(filePath)) return Results.NotFound();
    return Results.File(filePath, "image/png");
}).WithMetadata(new SignAuthorizeAttribute());

// Apply HTTP transport protections to the MCP endpoint while leaving image delivery public.
if (!string.IsNullOrEmpty(apiKey) || allowedOrigins.Count > 0)
{
    app.Use(async (context, next) =>
    {
        if (context.Request.Path.StartsWithSegments("/images")
            || context.Request.Path.StartsWithSegments("/health"))
        {
            await next();
            return;
        }

        var origin = context.Request.Headers.Origin.FirstOrDefault();
        if (!string.IsNullOrEmpty(origin)
            && allowedOrigins.Count > 0
            && !allowedOrigins.Contains(origin))
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Origin not allowed");
            return;
        }

        if (string.IsNullOrEmpty(apiKey))
        {
            await next();
            return;
        }

        var key = context.Request.Headers["X-API-Key"].FirstOrDefault()
               ?? context.Request.Query["api_key"].FirstOrDefault()
               ?? context.Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();
        if (key != apiKey)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }
        await next();
    });
}

app.MapMcp();

app.Run();

