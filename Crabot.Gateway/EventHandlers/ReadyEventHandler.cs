using System.Linq;
using System.Threading.Tasks;
using Crabot.Contracts;
using Crabot.Core.Events;
using Crabot.Core.Repositories;
using Crabot.MessageExtensions;
using Crabot.Rest.RestClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Crabot.Gateway.EventHandlers
{
    public class ReadyEventHandler : IGatewayEventHandler<ReadyEvent>
    {
        private readonly IDiscordRestClient _discordRestClient;
        private readonly IClientInfoRepository _clientInfoRepository;

        public ReadyEventHandler(
            IDiscordRestClient discordRestClient,
            IClientInfoRepository clientInfoRepository)
        {
            _discordRestClient = discordRestClient;
            _clientInfoRepository = clientInfoRepository;
        }

        public async Task HandleAsync(object @event)
        {
            if (_clientInfoRepository.GetClientInfo() != null)
            {
                _clientInfoRepository.DeleteClientInfo();
            }

            _clientInfoRepository.AddClientInfo(JsonConvert.DeserializeObject<ClientInfo>(@event.ToString()));

            await _discordRestClient.PostMessage("764840399696822322", "Ready to serve");
        }
    }
}
