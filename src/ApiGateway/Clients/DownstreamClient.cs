using System.Net.Http;

namespace ApiGateway.Clients;

public sealed class DownstreamClient : IDownstreamClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DownstreamClient> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DownstreamClient(
        HttpClient httpClient,
        ILogger<DownstreamClient> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> GetDataAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Calling downstream service");

        var correlationId = _httpContextAccessor
            .HttpContext?
            .Request
            .Headers["X-Correlation-ID"]
            .ToString();

        var request = new HttpRequestMessage(HttpMethod.Get, "/data");

        if (!string.IsNullOrWhiteSpace(correlationId))
        {
            request.Headers.Add("X-Correlation-ID", correlationId);
            _logger.LogInformation("Propagating Correlation ID: {CorrelationId}", correlationId);
        }

        var response = await _httpClient.SendAsync(request, cancellationToken);

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
