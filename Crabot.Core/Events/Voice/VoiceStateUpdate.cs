using Newtonsoft.Json;

namespace Crabot.Core.Events.Voice
{
    public class VoiceStateUpdate
    {
        [JsonProperty("member")]
        public Member Member { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("suppress")]
        public bool Suppress { get; set; }

        [JsonProperty("session_id")]
        public string SessionId { get; set; }

        [JsonProperty("self_video")]
        public bool SelfVideo { get; set; }

        [JsonProperty("self_mute")]
        public bool SelfMute { get; set; }

        [JsonProperty("self_deaf")]
        public bool SelfDeaf { get; set; }

        [JsonProperty("request_to_speak_timestamp")]
        public string RequestToSpeakTimestamp { get; set; }

        [JsonProperty("mute")]
        public bool Mute { get; set; }

        [JsonProperty("guild_id")]
        public string GuildId { get; set; }

        [JsonProperty("deaf")]
        public bool Deaf { get; set; }

        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }
    }
}
