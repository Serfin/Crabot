using System.Collections.Generic;
using Newtonsoft.Json;

namespace Crabot.Contracts
{
    public class Guild
    {
        [JsonProperty("description")]
        public object Description { get; set; }

        [JsonProperty("member_count")]
        public int MemberCount { get; set; }

        [JsonProperty("system_channel_id")]
        public string SystemChannelId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("preferred_locale")]
        public string PreferredLocale { get; set; }

        [JsonProperty("splash")]
        public object Splash { get; set; }

        [JsonProperty("owner_id")]
        public string OwnerId { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("emojis")]
        public List<Emoji> Emojis { get; set; }

        [JsonProperty("roles")]
        public List<Role> Roles { get; set; }

        [JsonProperty("channels")]
        public List<Channel> Channels { get; set; }
    }
}
