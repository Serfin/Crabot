using System.Threading.Tasks;
using Crabot.Core.Events;
using Crabot.Rest.RestClient;
using Crabot.WebSocket;
using Newtonsoft.Json;

namespace Crabot.Gateway.EventHandlers
{
    public class HelloEventHandler : IGatewayEventHandler<HeartbeatEvent>
    {
        private readonly IDiscordRestClient _discordRestClient;
        private readonly IConnectionManager _connectionManager;

        public HelloEventHandler(IDiscordRestClient discordRestClient, 
            IConnectionManager connectionManager)
        {
            _discordRestClient = discordRestClient;
            _connectionManager = connectionManager;
        }

        public async Task HandleAsync(object eventData)
        {
            var heartbeatInterval = JsonConvert.DeserializeObject<HeartbeatEvent>(eventData.ToString())
                .HeartbeatInterval;

            await _discordRestClient.PostMessage("764840399696822322", string.Format(
                "```json\n [DEBUG C -> S] Client started heartbeating! \n\n {0} ```", eventData.ToString()));

        }
    }
}
