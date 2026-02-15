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

app.MapGet("/health", () => Results.Ok("Downstream healthy"));

app.Run();
