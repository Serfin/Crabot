using Newtonsoft.Json;

namespace Crabot.Core.Events.Voice
{
    public class VoiceServerUpdate
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("guild_id")]
        public string GuildId { get; set; }

        [JsonProperty("endpoint")]
        public string Endpoint { get; set; }
    }
}
