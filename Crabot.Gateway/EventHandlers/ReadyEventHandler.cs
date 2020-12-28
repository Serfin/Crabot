using System.Threading.Tasks;
using Crabot.Contracts;
using Crabot.Core.Events;
using Crabot.Core.Repositories;
using Newtonsoft.Json;

namespace Crabot.Gateway.EventHandlers
{
    public class ReadyEventHandler : IGatewayEventHandler<ReadyEvent>
    {
        private readonly IClientInfoRepository _clientInfoRepository;

        public ReadyEventHandler(IClientInfoRepository clientInfoRepository)
        {
            _clientInfoRepository = clientInfoRepository;
        }

        public async Task HandleAsync(object @event)
        {
            if (_clientInfoRepository.GetClientInfo() != null)
            {
                _clientInfoRepository.DeleteClientInfo();
            }

            _clientInfoRepository.AddClientInfo(JsonConvert.DeserializeObject<ClientInfo>(@event.ToString()));

            await Task.CompletedTask;
        }
    }
}
