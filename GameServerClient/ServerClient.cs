using GameServerClient.Contracts.Requests;
using GameServerClient.Contracts.Responses;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace GameServerClient
{
    internal class ServerClient : IAsyncDisposable
    {
        private readonly HttpClient httpClient;
        private readonly string masterServerUrl;
        private HubConnection? hubConnection;
        private string? serverId;
        private string? authToken;

        public ServerClient(string masterServerUrl)
        {
            this.masterServerUrl = masterServerUrl;
            httpClient = new HttpClient { BaseAddress = new Uri(masterServerUrl) };
        }

        public async Task RegisterServerAsync(string serverName, string host, uint port)
        {
            // Step 1: Register the server
            var response = await httpClient.PostAsJsonAsync("/api/register", new RegisterServerRequest
            {
                Name = serverName,
                Host = host,
                Port = port
            });

            var result = await response.Content.ReadFromJsonAsync<RegisterServerResponse>();

            serverId = result!.ServerID;
            authToken = result.AuthToken;

            await Task.Delay(200); // Small delay to ensure server is ready
            // Step 2: Connect to SignalR hub
            hubConnection = new HubConnectionBuilder()
                .WithUrl($"{masterServerUrl}/hubs/gameserver?serverId={serverId}&token={authToken}")
                .WithAutomaticReconnect()
                .Build();

            await hubConnection.StartAsync();
        }

        public bool IsConnected => hubConnection?.State == HubConnectionState.Connected;
        public async Task UpdateStatusAsync(string map, int currentPlayers, int maxPlayers)
        {
            if (hubConnection?.State != HubConnectionState.Connected)
                throw new InvalidOperationException("Not connected to master server");

            await hubConnection.SendAsync("UpdateStatus", new GameInfoUpdateRequest
            {
                ServerID = serverId,
                AuthToken = authToken,
                Map = map,
                CurrentPlayers = currentPlayers,
                MaxPlayers = maxPlayers
            });
        }

        public async ValueTask DisposeAsync()
        {
            if (hubConnection is not null)
                await hubConnection.DisposeAsync();
            httpClient.Dispose();
        }
    }
}
