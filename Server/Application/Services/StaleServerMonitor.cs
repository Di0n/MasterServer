using Microsoft.AspNetCore.SignalR;

namespace Server.Application.Services
{
    public class StaleServerMonitor : BackgroundService
    {
        private readonly IGameServerManager serverManager;
        private readonly IHubContext<GameServerHub> hubContext;
        private readonly double staleServerMonitorInterval;

        public StaleServerMonitor(IGameServerManager serverManager, IHubContext<GameServerHub> hubContext, IConfiguration configuration)
        {
            this.serverManager = serverManager;
            this.hubContext = hubContext;
            staleServerMonitorInterval = configuration.GetValue<double>("MasterServer:StaleServerMonitorInterval", 10.0);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                serverManager.UnregisterInactiveServers();
                //var now = DateTime.UtcNow;
                //foreach (var kvp in serverManager.server)
                //{
                //    var server = kvp.Value;
                //    if ((now - server.LastHeartbeat).TotalSeconds > 30)
                //    {
                //        // Remove from manager
                //        serverManager.UnregisterServer(server.Id);

                //        // Disconnect SignalR connection if possible
                //        var connectionId = serverManager.connectionIdToServerId
                //            .FirstOrDefault(x => x.Value == server.Id).Key;
                //        if (!string.IsNullOrEmpty(connectionId))
                //        {
                //            await hubContext.Clients.Client(connectionId).SendAsync("Disconnect", cancellationToken: stoppingToken);
                //        }
                //    }
                //}
                await Task.Delay(TimeSpan.FromSeconds(staleServerMonitorInterval), stoppingToken);
            }
        }
    }
}
