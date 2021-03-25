using System;
using System.Collections.Generic;
using Crabot.Core.Events.NestedTypes;
using Newtonsoft.Json;

namespace Crabot.Core.Events
{
    public class MessageReactionAdd : IGatewayEvent

    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("message_id")]
        public string MessageId { get; set; }

        [JsonProperty("member")]
        public Member Member { get; set; }

        [JsonProperty("emoji")]
        public Emoji Emoji { get; set; }

        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }

        [JsonProperty("guild_id")]
        public string GuildId { get; set; }
    }

    public class Member
    {
        [JsonProperty("user")]
        public NestedTypes.User User { get; set; }

        [JsonProperty("roles")]
        public List<string> Roles { get; set; }

        [JsonProperty("mute")]
        public bool Mute { get; set; }

        [JsonProperty("joined_at")]
        public DateTime JoinedAt { get; set; }

        [JsonProperty("hoisted_role")]
        public string HoistedRole { get; set; }

        [JsonProperty("deaf")]
        public bool Deaf { get; set; }
    }
}
