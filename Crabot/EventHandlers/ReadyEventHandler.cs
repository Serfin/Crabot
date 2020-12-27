using System;
using System.Threading.Tasks;
using Crabot.Contracts;
using Crabot.Models;
using Crabot.Repositories;
using Newtonsoft.Json;

namespace Crabot.EventHandlers
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
            _clientInfoRepository.AddClientInfo(JsonConvert.DeserializeObject<ClientInfo>(@event.ToString()));

            await Task.CompletedTask;
        }
    }
}
