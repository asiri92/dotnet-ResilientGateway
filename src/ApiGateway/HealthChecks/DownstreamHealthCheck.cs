using Microsoft.Extensions.Diagnostics.HealthChecks;

public class DownstreamHealthCheck : IHealthCheck
{
    private readonly IHttpClientFactory _factory;

    public DownstreamHealthCheck(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var client = _factory.CreateClient("downstream");

        try
        {
            var response = await client.GetAsync("/health", cancellationToken);

            if (response.IsSuccessStatusCode)
                return HealthCheckResult.Healthy("Downstream reachable");

            return HealthCheckResult.Unhealthy("Downstream unhealthy");
        }
        catch
        {
            return HealthCheckResult.Unhealthy("Downstream unreachable");
        }
    }
}