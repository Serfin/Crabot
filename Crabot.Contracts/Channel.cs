using System.Collections.Generic;
using Newtonsoft.Json;

namespace Crabot.Contracts
{
    public class Channel
    {
        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("position")]
        public int Position { get; set; }

        [JsonProperty("permission_overwrites")]
        public List<object> PermissionOverwrites { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
