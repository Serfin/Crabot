using System.Text;
using System.Threading.Tasks;
using Crabot.Gateway;
using Crabot.Gateway.SocketClient;
using Crabot.Models;
using Crabot.Repositories;
using Crabot.Rest.RestClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Crabot.EventHandlers
{
    public class ResumeEventHandler : IGatewayEventHandler<ResumeEvent>
    {
        private readonly IDiscordSocketClient _discordSocketClient;
        private readonly IDiscordRestClient _discordRestClient;
        private readonly IClientInfoRepository _clientInfoRepository;
        private readonly IConfiguration _configuration;

        public ResumeEventHandler(
            IDiscordSocketClient discordSocketClient, 
            IDiscordRestClient discordRestClient,
            IClientInfoRepository clientInfoRepository, 
            IConfiguration configuration)
        {
            _discordSocketClient = discordSocketClient;
            _discordRestClient = discordRestClient;
            _clientInfoRepository = clientInfoRepository;
            _configuration = configuration;
        }

        public async Task HandleAsync(object @event)
        {
            _discordSocketClient.RequestCancellation();
            var gatewayUrl = await _discordRestClient.GetGatewayUrlAsync();
            await _discordSocketClient.ConnectAsync(gatewayUrl);

            var resumeEvent = new GatewayPayload
            {
                Opcode = GatewayOpCode.Resume,
                EventData = new ResumeEvent
                {
                    Token = _configuration["botSecret"],
                    SessionId = _clientInfoRepository.GetClientInfo().SessionId,
                    Sequence = _discordSocketClient.SequenceNumber.Value
                }
            };

            var resumeEventBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(resumeEvent));
            await _discordSocketClient.SendAsync(resumeEventBytes, true);
        }
    }
}
