using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Crabot.Contracts
{
    public class GatewayMessage
    {
        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("tts")]
        public bool Tts { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("referenced_message")]
        public object ReferencedMessage { get; set; }

        [JsonProperty("pinned")]
        public bool Pinned { get; set; }

        [JsonProperty("nonce")]
        public string Nonce { get; set; }

        [JsonProperty("mentions")]
        public List<Mention> Mentions { get; set; }

        [JsonProperty("mention_roles")]
        public List<string> MentionRoles { get; set; }

        [JsonProperty("mention_everyone")]
        public bool MentionEveryone { get; set; }

        [JsonProperty("member")]
        public Member Member { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("flags")]
        public int Flags { get; set; }

        [JsonProperty("embeds")]
        public List<object> Embeds { get; set; }

        [JsonProperty("edited_timestamp")]
        public object EditedTimestamp { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }

        [JsonProperty("author")]
        public Author Author { get; set; }

        [JsonProperty("attachments")]
        public List<object> Attachments { get; set; }

        [JsonProperty("guild_id")]
        public string GuildId { get; set; }
    }
}
