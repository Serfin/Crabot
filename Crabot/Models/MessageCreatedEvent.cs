using Newtonsoft.Json;

namespace Crabot
{
    public class MessageCreatedEvent : IGatewayEvent
    {
        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }

        [JsonProperty("guild_id")]
        public string GuildId { get; set; }
    }
}
