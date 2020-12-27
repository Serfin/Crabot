using Newtonsoft.Json;

namespace Crabot.Rest.Models
{
    public class Message
    {
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
