using System;
using System.Threading.Tasks;
using Crabot.Rest.RestClient;
using Crabot.WebSocket;

namespace Crabot.Gateway
{
    public class DiscordGatewayClient : IDiscordGatewayClient
    {
        private readonly IConnectionManager _connectionManager;
        private readonly IDiscordRestClient _discordRestClient;
        private readonly IGatewayEventDispatcher _gatewayDiscordDispatcher;

        public DiscordGatewayClient(
            IConnectionManager connectionManager,
            IDiscordRestClient discordRestClient,
            IGatewayEventDispatcher gatewayDiscordDispatcher)
        {
            _connectionManager = connectionManager;
            _discordRestClient = discordRestClient;
            _gatewayDiscordDispatcher = gatewayDiscordDispatcher;
        }

        public async Task StartAsync()
        {
            var gatewayUrl = await _discordRestClient.GetGatewayUrlAsync();

            _connectionManager.EventReceive += _gatewayDiscordDispatcher.DispatchEvent;
            await _connectionManager.CreateConnectionAsync(new Uri(gatewayUrl));
        }
    }
}
