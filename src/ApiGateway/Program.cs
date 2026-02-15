using ApiGateway.Clients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();

// Register HttpClient with base address
builder.Services.AddHttpClient<IDownstreamClient, DownstreamClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5074");
    // ⚠️ Replace port if your downstream runs on different port
});

var app = builder.Build();

app.MapGet("/gateway/data", async (
    IDownstreamClient client,
    CancellationToken cancellationToken) =>
{
    var result = await client.GetDataAsync(cancellationToken);

    return Results.Ok(result);
});

app.Run();
