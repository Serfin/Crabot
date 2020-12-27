using System.Collections.Generic;
using Newtonsoft.Json;

namespace Crabot.Contracts
{
    public class Emoji
    {
        [JsonProperty("roles")]
        public List<object> Roles { get; set; }

        [JsonProperty("require_colons")]
        public bool RequireColons { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("managed")]
        public bool Managed { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("available")]
        public bool Available { get; set; }

        [JsonProperty("animated")]
        public bool Animated { get; set; }
    }
}
