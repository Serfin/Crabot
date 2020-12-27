using Newtonsoft.Json;

namespace Crabot.Contracts
{
    public class Role
    {
        [JsonProperty("position")]
        public int Position { get; set; }

        [JsonProperty("permissions")]
        public string Permissions { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("mentionable")]
        public bool Mentionable { get; set; }

        [JsonProperty("managed")]
        public bool Managed { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("hoist")]
        public bool Hoist { get; set; }

        [JsonProperty("color")]
        public int Color { get; set; }
    }
}
