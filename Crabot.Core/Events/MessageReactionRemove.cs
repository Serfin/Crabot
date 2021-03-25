using Crabot.Core.Events.NestedTypes;
using Newtonsoft.Json;

namespace Crabot.Core.Events
{
    public class MessageReactionRemove : IGatewayEvent
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("message_id")]
        public string MessageId { get; set; }

        [JsonProperty("emoji")]
        public Emoji Emoji { get; set; }

        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }

        [JsonProperty("guild_id")]
        public string GuildId { get; set; }
    }
}