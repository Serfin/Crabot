using System.Threading.Tasks;
using Crabot.Core.Events;
using Crabot.Rest.Models;
using Crabot.Rest.RestClient;

namespace Crabot.Gateway.EventHandlers
{
    public class HelloEventHandler : IGatewayEventHandler<HeartbeatEvent>
    {
        private readonly IDiscordRestClient _discordRestClient;

        public HelloEventHandler(IDiscordRestClient discordRestClient)
        {
            _discordRestClient = discordRestClient;
        }

        public async Task HandleAsync(object eventData)
        {
            await _discordRestClient.PostMessage("764840399696822322",
                new Message
                {
                    Content = string.Format("```json\n [DEBUG C -> S] Client started heartbeating! \n\n {0} ```", eventData.ToString())
                });

        }
    }
}
