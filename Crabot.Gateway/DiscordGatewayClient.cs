using System.Threading.Tasks;
using Crabot.WebSocket;

namespace Crabot.Gateway
{
    public class DiscordGatewayClient : IDiscordGatewayClient
    {
        private readonly IConnectionManager _connectionManager;
        private readonly IGatewayEventDispatcher _gatewayDiscordDispatcher;

        public DiscordGatewayClient(
            IConnectionManager connectionManager,
            IGatewayEventDispatcher gatewayDiscordDispatcher)
        {
            _connectionManager = connectionManager;
            _gatewayDiscordDispatcher = gatewayDiscordDispatcher;
        }

        public async Task StartAsync()
        {
            _connectionManager.EventReceive += _gatewayDiscordDispatcher.DispatchEvent;
            await _connectionManager.CreateConnectionAsync();
        }
    }
}
