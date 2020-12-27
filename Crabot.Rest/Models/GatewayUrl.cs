using Newtonsoft.Json;

namespace Crabot.Rest.Models
{
    public class GatewayUrl
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
