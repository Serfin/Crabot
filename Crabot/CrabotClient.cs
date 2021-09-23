using System.Net.WebSockets;
using System.Threading.Tasks;
using Autofac;
using Crabot.Gateway;
using Crabot.WebSocket;

namespace Crabot
{
    public class CrabotClient
    {
        private readonly IComponentContext _context;
        private readonly IGatewayDispatcher _gatewayDiscordDispatcher;

        public CrabotClient(
            IComponentContext context,
            IGatewayDispatcher gatewayDiscordDispatcher)
        {
            _context = context;
            _gatewayDiscordDispatcher = gatewayDiscordDispatcher;
        }

        public async Task StartAsync()
        {
            var eventConnection = _context.ResolveNamed<IGatewayConnection>("event-gateway-connection");

            eventConnection.EventReceive += _gatewayDiscordDispatcher.DispatchEvent;
            await eventConnection.StartConnectionAsync();
        }
    }
}
