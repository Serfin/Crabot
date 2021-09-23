using System.Text;
using System.Threading.Tasks;
using Crabot.Core.Events;
using Crabot.Core.Events.Voice;
using Crabot.Rest.RestClient;
using Crabot.WebSocket;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Crabot.Voice
{
    public class VoiceClient : IVoiceClient
    {
        //        private readonly ILogger _logger;

        //        public VoiceClient(
        //            ILogger<VoiceClient> logger,
        //            IDiscordSocketClient discordSocketClient)
        //        {
        //            _logger = logger;
        //        }

        //        public async Task JoinChannel(string guildId, string channelId)
        //        {
        //            var voiceState = new GatewayPayload
        //            {
        //                Opcode = (int)GatewayOpCode.VoiceStateUpdate,
        //                EventData = new VoiceStateUpdate
        //                {
        //                    GuildId = guildId,
        //                    ChannelId = channelId,
        //                    SelfDeaf = false,
        //                    SelfMute = false
        //                }
        //            };

        //            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(voiceState));

        //            await _discordSocketClient.SendAsync(bytes, true);
        //        }

        //        public void SetVoiceStateUpdate(VoiceStateUpdate voiceStateUpdate)
        //        {
        //            _voiceStateUpdate = voiceStateUpdate;
        //        }

        //        public async Task SetVoiceServerUpdateAsync(VoiceServerUpdate voiceServerUpdate)
        //        {
        //            _voiceServerUpdate = voiceServerUpdate;
        //            await TryConnectAsync();
        //        }

        //        public async Task TryConnectAsync()
        //        {
        //            var identifyEvent = new GatewayPayload
        //            {
        //                Opcode = (int)GatewayVoiceOpCode.Identify,
        //                EventData = new VoiceIdentify
        //                {
        //                    UserId = _voiceStateUpdate.UserId,
        //                    ServerId = _voiceStateUpdate.GuildId,
        //                    SessionId = _voiceStateUpdate.SessionId,
        //                    Token = _voiceServerUpdate.Token
        //                }
        //            };

        //            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(identifyEvent));

        //            await _discordSocketClient.SendAsync(bytes, true);
        //        }
    }
}
