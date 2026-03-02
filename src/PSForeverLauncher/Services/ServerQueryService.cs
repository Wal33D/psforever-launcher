using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using PSForeverLauncher.Models;

namespace PSForeverLauncher.Services;

public sealed class ServerQueryService : IServerQueryService, IDisposable
{
    private static readonly TimeSpan ConnectionTimeout = TimeSpan.FromSeconds(5);
    private readonly ConcurrentDictionary<string, ServerStatus> _cache = new();
    private CancellationTokenSource? _pollCts;
    private int _pollIntervalSeconds = 15;

    public event Action<ServerEntry, ServerStatus>? StatusUpdated;

    public ServerStatus GetStatus(ServerEntry server) =>
        _cache.GetValueOrDefault(server.Id, ServerStatus.CreateUnknown());

    public async Task QueryServerAsync(ServerEntry server, CancellationToken ct = default)
    {
        try
        {
            using var client = new TcpClient();
            using var connectCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            connectCts.CancelAfter(ConnectionTimeout);

            await client.ConnectAsync(server.Host, server.AdminPort, connectCts.Token);

            var stream = client.GetStream();
            stream.ReadTimeout = (int)ConnectionTimeout.TotalMilliseconds;
            stream.WriteTimeout = (int)ConnectionTimeout.TotalMilliseconds;

            var command = Encoding.UTF8.GetBytes("list_players\n");
            await stream.WriteAsync(command, ct);

            var buffer = new byte[8192];
            var responseBuilder = new StringBuilder();
            var readCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            readCts.CancelAfter(ConnectionTimeout);

            try
            {
                int bytesRead;
                while ((bytesRead = await stream.ReadAsync(buffer, readCts.Token)) > 0)
                {
                    responseBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                    if (responseBuilder.ToString().Contains('\n'))
                        break;
                }
            }
            catch (OperationCanceledException) when (!ct.IsCancellationRequested)
            {
                // Read timeout — use what we have
            }

            var response = responseBuilder.ToString().Trim();
            var status = ParseResponse(response);
            _cache[server.Id] = status;
            StatusUpdated?.Invoke(server, status);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            var status = new ServerStatus
            {
                State = ServerConnectionState.Unknown,
                ErrorMessage = ex is SocketException ? "Server unreachable" : ex.Message
            };
            _cache[server.Id] = status;
            StatusUpdated?.Invoke(server, status);
        }
    }

    public void StartPolling(IEnumerable<ServerEntry> servers, CancellationToken ct = default)
    {
        StopPolling();
        _pollCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        var token = _pollCts.Token;
        var serverList = servers.ToList();

        _ = Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                var tasks = serverList.Select(s => QueryServerAsync(s, token));
                try
                {
                    await Task.WhenAll(tasks);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(_pollIntervalSeconds), token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }, token);
    }

    public void StopPolling()
    {
        _pollCts?.Cancel();
        _pollCts?.Dispose();
        _pollCts = null;
    }

    public void SetPollInterval(int seconds) => _pollIntervalSeconds = Math.Max(5, seconds);

    private static ServerStatus ParseResponse(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
            return ServerStatus.CreateOffline("Empty response");

        try
        {
            var parsed = JsonSerializer.Deserialize<ListPlayersResponse>(response);
            if (parsed is null)
                return ServerStatus.CreateOffline("Invalid response");

            var players = parsed.Players.Select(p => new PlayerInfo
            {
                Name = p.Name,
                Faction = ParseFaction(p.Faction),
                CharId = p.CharId
            }).ToList();

            return new ServerStatus
            {
                State = ServerConnectionState.Online,
                PlayerCount = parsed.Count,
                Players = players
            };
        }
        catch (JsonException)
        {
            // Might be a non-JSON text response — server is reachable but not returning expected format
            return new ServerStatus
            {
                State = ServerConnectionState.Online,
                ErrorMessage = "Unexpected response format"
            };
        }
    }

    private static Faction ParseFaction(string faction) => faction.ToUpperInvariant() switch
    {
        "TR" => Faction.TR,
        "NC" => Faction.NC,
        "VS" => Faction.VS,
        _ => Faction.Unknown
    };

    public void Dispose() => StopPolling();
}
