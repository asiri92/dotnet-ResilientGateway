namespace ApiGateway.Clients;

public interface IDownstreamClient
{
    Task<string> GetDataAsync(CancellationToken cancellationToken);
}
