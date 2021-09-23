using System;
using System.Threading.Tasks;
using Crabot.Core.Events.Voice;
using Crabot.Voice;
using Newtonsoft.Json;

namespace Crabot.Gateway.EventHandlers
{
    public class VoiceStateUpdateEventHandler : IGatewayEventHandler<VoiceStateUpdate>
    {
        private VoiceClient _voiceClient;
        public VoiceStateUpdateEventHandler(VoiceClient voiceClient)
        {
            _voiceClient = voiceClient;
        }

        public async Task HandleAsync(object @event)
        {
            var parsedEvent = JsonConvert.DeserializeObject<VoiceStateUpdate>(@event.ToString());

            //_voiceClient.SetVoiceStateUpdate(parsedEvent);

            await Task.CompletedTask;
        }
    }
}
