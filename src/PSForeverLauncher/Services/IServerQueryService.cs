using PSForeverLauncher.Models;

namespace PSForeverLauncher.Services;

public interface IServerQueryService
{
    event Action<ServerEntry, ServerStatus>? StatusUpdated;
    ServerStatus GetStatus(ServerEntry server);
    Task QueryServerAsync(ServerEntry server, CancellationToken ct = default);
    void StartPolling(IEnumerable<ServerEntry> servers, CancellationToken ct = default);
    void StopPolling();
}
