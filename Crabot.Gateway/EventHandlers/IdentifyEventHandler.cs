using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Crabot.Core.Events;
using Crabot.Rest.Models;
using Crabot.Rest.RestClient;
using Crabot.WebSocket;
using Newtonsoft.Json;

namespace Crabot.Gateway.EventHandlers
{
    public class IdentifyEventHandler : IGatewayEventHandler<IdentifyEvent>
    {
        private readonly IDiscordRestClient _discordRestClient;
        private readonly IDiscordSocketClient _discordSocketClient;

        public IdentifyEventHandler(IDiscordRestClient discordRestClient, 
            IDiscordSocketClient discordSocketClient)
        {
            _discordRestClient = discordRestClient;
            _discordSocketClient = discordSocketClient;
        }

        public async Task HandleAsync(object @event)
        {
            var identityEvent = new GatewayPayload
            {
                Opcode = GatewayOpCode.Identify,
                EventData = new IdentifyEvent
                {
                    Token = Environment.GetEnvironmentVariable("BOT_TOKEN"),
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

            await _discordRestClient.PostMessage("764840399696822322",
                new Message { Content = "``` [DEBUG C -> S] identifying as Crabot! ```" });
        }
    }
}
