using System;
using System.Threading.Tasks;
using Crabot.Core.Events;
using Crabot.Rest.RestClient;
using Crabot.WebSocket;

namespace Crabot.Gateway.EventHandlers
{
    public class ReconnectEventHandler : IGatewayEventHandler<ReconnectEvent>
    {
        private readonly IDiscordRestClient _discordRestClient;
        private readonly IConnectionManager _connectionManager;

        public ReconnectEventHandler(
            IDiscordRestClient discordRestClient, 
            IConnectionManager connectionManager)
        {
            _discordRestClient = discordRestClient;
            _connectionManager = connectionManager;
        }

        public async Task HandleAsync(object @event)
        {
            var gatewayUrl = await _discordRestClient.GetGatewayUrlAsync();
            await _connectionManager.CreateConnectionAsync(new Uri(gatewayUrl));
        }
    }
}
