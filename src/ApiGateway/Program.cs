using ApiGateway.Clients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();

builder.Services
    .AddHttpClient<IDownstreamClient, DownstreamClient>(client =>
    {
        client.BaseAddress = new Uri("http://localhost:5074"); // adjust port
    })
    .AddStandardResilienceHandler(); ;

var app = builder.Build();

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

app.Run();
