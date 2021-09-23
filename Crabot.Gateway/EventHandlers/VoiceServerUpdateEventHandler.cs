using System.Threading.Tasks;
using Crabot.Core.Events.Voice;
using Crabot.Voice;
using Newtonsoft.Json;

namespace Crabot.Gateway.EventHandlers
{
    public class VoiceServerUpdateEventHandler : IGatewayEventHandler<VoiceServerUpdate>
    {
        private VoiceClient _voiceClient;
        public VoiceServerUpdateEventHandler(VoiceClient voiceClient)
        {
            _voiceClient = voiceClient;
        }

        public async Task HandleAsync(object @event)
        {
            var parsedEvent = JsonConvert.DeserializeObject<VoiceServerUpdate>(@event.ToString());

            //await _voiceClient.SetVoiceServerUpdateAsync(parsedEvent);
        }
    }
}
