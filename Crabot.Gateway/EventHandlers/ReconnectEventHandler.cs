using System;
using System.Threading.Tasks;
using Crabot.Core.Events;
using Crabot.Rest.Models;
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
            await _discordRestClient.PostMessage("764840399696822322",
                new Message { Content = "```[DEBUG C <- S] Server requested reconnect! ```" });

            var gatewayUrl = await _discordRestClient.GetGatewayUrlAsync();
            await _connectionManager.CreateConnectionAsync(new Uri(gatewayUrl));
        }
    }
}
