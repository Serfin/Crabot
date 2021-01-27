using System.Threading.Tasks;
using Crabot.Contracts;
using Crabot.Core.Events;
using Crabot.Core.Repositories;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Crabot.Gateway.EventHandlers
{
    public class ReadyEventHandler : IGatewayEventHandler<ReadyEvent>
    {
        private readonly IClientInfoRepository _clientInfoRepository;
        private readonly ILogger _logger;

        public ReadyEventHandler(IClientInfoRepository clientInfoRepository, 
            ILogger<ReadyEventHandler> logger)
        {
            _clientInfoRepository = clientInfoRepository;
            _logger = logger;
        }

        public async Task HandleAsync(object @event)
        {
            if (_clientInfoRepository.GetClientInfo() != null)
            {
                _clientInfoRepository.DeleteClientInfo();
            }

            _clientInfoRepository.AddClientInfo(JsonConvert.DeserializeObject<ClientInfo>(@event.ToString()));

            _logger.LogInformation("Session Id - {0}", _clientInfoRepository.GetClientInfo().SessionId);

            await Task.CompletedTask;
        }
    }
}
