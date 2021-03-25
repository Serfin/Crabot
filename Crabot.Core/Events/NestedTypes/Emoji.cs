using Newtonsoft.Json;

namespace Crabot.Core.Events.NestedTypes
{
    public class Emoji
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
