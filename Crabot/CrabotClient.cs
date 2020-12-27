using System.Threading.Tasks;
using Crabot.Gateway.SocketClient;
using Crabot.Rest.RestClient;
using Newtonsoft.Json;

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
            var gatewayUrl = await _discordRestClient.GetGatewayUrlAsync();

            _discordSocketClient.TextMessage += GatewayTextMessage;
            await _discordSocketClient.ConnectAsync(gatewayUrl);
        }

        private async Task GatewayTextMessage(Gateway.GatewayPayload gatewayPayload)
        {
            if (gatewayPayload.SequenceNumber.HasValue)
            {
                _discordSocketClient.SequenceNumber = gatewayPayload.SequenceNumber;
            }
            else
            {
                gatewayPayload.SequenceNumber = _discordSocketClient.SequenceNumber;
            }

            await _eventDispatcher.DispatchEvent(gatewayPayload);
        }
    }
}
