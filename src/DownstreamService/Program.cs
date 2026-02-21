var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();

var app = builder.Build();

var random = new Random();

app.MapGet("/data", async (ILogger<Program> logger) =>
{
    var delay = random.Next(100, 1500);
    await Task.Delay(delay);

    var shouldFail = random.NextDouble() < 0.3; // 30% failure rate

    if (shouldFail)
    {
        logger.LogWarning("Downstream failure simulated");
        return Results.StatusCode(500);
    }

    logger.LogInformation("Downstream success after {Delay}ms", delay);

    return Results.Ok(new
    {
        message = "Downstream response",
        delay
    });
});

// Test RetryGatewayHandler by always returning 500
//app.MapGet("/data", () =>
//{
//    Console.WriteLine($"Downstream called at {DateTime.Now:HH:mm:ss.fff}");
//    return Results.StatusCode(500);
//});

app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        status = "Healthy",
        timestamp = DateTime.UtcNow
    });
});

app.Run();
