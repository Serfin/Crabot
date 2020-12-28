using System.Collections.Generic;
using Newtonsoft.Json;

namespace Crabot.Core.Events
{
    public class IdentifyEvent : IGatewayEvent
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("properties")]
        public IDictionary<string, string> Properties { get; set; }

        [JsonProperty("intents")]
        public int? Intents { get; set; }

        [JsonProperty("compress")]
        public bool Compress { get; set; }
    }
}
