using System.Threading.Tasks;
using Crabot.Gateway.SocketClient;
using Crabot.Rest.RestClient;

namespace Crabot
{
    public class CrabotClient
    {
        private readonly IDiscordRestClient _discordRestClient;
        private readonly IDiscordSocketClient _discordSocketClient;
        private readonly IEventDispatcher _eventDispatcher;

        public CrabotClient(
            IDiscordRestClient discordRestClient,
            IDiscordSocketClient discordSocketClient,
            IEventDispatcher eventDispatcher)
        {
            _discordRestClient = discordRestClient;
            _discordSocketClient = discordSocketClient;
            _eventDispatcher = eventDispatcher;
        }

        public async Task StartAsync()
        {
            _discordSocketClient.MessageReceive += _eventDispatcher.DispatchEvent;

            var gatewayUrl = await _discordRestClient.GetGatewayUrlAsync();
            await _discordSocketClient.ConnectAsync(gatewayUrl);
        }
    }
}
