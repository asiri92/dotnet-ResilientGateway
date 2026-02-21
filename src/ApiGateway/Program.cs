using ApiGateway.Clients;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddHttpContextAccessor();

builder.Services.AddHealthChecks()
    .AddCheck<DownstreamHealthCheck>(
        "downstream",
        tags: new[] { "ready" });

builder.Services
    .AddHttpClient<IDownstreamClient, DownstreamClient>(client =>
    {
        client.BaseAddress = new Uri("http://localhost:5074"); // adjust port
    })
    .AddStandardResilienceHandler(); ;

builder.Services.AddHttpClient("downstream", client =>
{
    client.BaseAddress = new Uri("http://localhost:5074");
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    const string headerName = "X-Correlation-ID";

    if (!context.Request.Headers.TryGetValue(headerName, out var correlationId))
    {
        correlationId = Guid.NewGuid().ToString();
        context.Request.Headers[headerName] = correlationId;
    }

    context.Response.Headers[headerName] = correlationId;

    using (context.RequestServices
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("Correlation")
        .BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId.ToString()
        }))
    {
        await next();
    }
});

app.Use(async (context, next) =>
{
    var logger = context.RequestServices
        .GetRequiredService<ILogger<Program>>();

    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

    await next();

    stopwatch.Stop();

    logger.LogInformation(
        "HTTP {Method} {Path} responded {StatusCode} in {Elapsed}ms",
        context.Request.Method,
        context.Request.Path,
        context.Response.StatusCode,
        stopwatch.ElapsedMilliseconds);
});

app.MapGet("/gateway/data", async (
    IDownstreamClient client,
    CancellationToken cancellationToken) =>
{
    try
    {
        var result = await client.GetDataAsync(cancellationToken);
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false // No checks, just app alive
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.Run();
