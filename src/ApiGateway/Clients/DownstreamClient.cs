using System.Net.Http;

namespace ApiGateway.Clients;

public sealed class DownstreamClient : IDownstreamClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DownstreamClient> _logger;

    public DownstreamClient(
        HttpClient httpClient,
        ILogger<DownstreamClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> GetDataAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Calling downstream service");

        var response = await _httpClient.GetAsync(
            "/data",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning(
                "Downstream returned {StatusCode}",
                response.StatusCode);

            throw new HttpRequestException(
                $"Downstream failed: {response.StatusCode}");
        }

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}
