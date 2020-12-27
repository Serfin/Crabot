using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Crabot.Gateway;
using Crabot.Gateway.SocketClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Crabot
{
    public class IdentifyEventHandler : IGatewayEventHandler<IdentifyEvent>
    {
        private readonly IConfiguration _configuration;
        private readonly IDiscordSocketClient _discordSocketClient;

        public IdentifyEventHandler(
            IConfiguration configuration, 
            IDiscordSocketClient discordSocketClient)
        {
            _discordSocketClient = discordSocketClient;
            _configuration = configuration;
        }

        public async Task HandleAsync(object @event)
        {
            var identityEvent = new GatewayPayload
            {
                Opcode = GatewayOpCode.Identify,
                EventData = new IdentifyEvent
                {
                    Token = _configuration["botSecret"],
                    Intents = 513,
                    Compress = false,
                    Properties = new Dictionary<string, string> {
                            { "$os", "windows" },
                            { "$browser", "crabot" },
                            { "$device", "crabot"}
                    }
                }
            };

            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(identityEvent));
            await _discordSocketClient.SendAsync(bytes, true);
        }
    }
}
