using Newtonsoft.Json;

namespace Crabot.Rest.Models
{
    public class RateLimit
    {
        [JsonProperty("global")]
        public bool Global { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("retry_after")]
        public float RetryAfter { get; set; }
    }
}
