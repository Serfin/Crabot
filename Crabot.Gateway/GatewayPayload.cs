using Newtonsoft.Json;

namespace Crabot.Gateway
{
    public class GatewayPayload
    {
        [JsonProperty("op")]
        public GatewayOpCode Opcode { get; set; }

        [JsonProperty("d")]
        public object EventData { get; set; }

        [JsonProperty("s", NullValueHandling = NullValueHandling.Ignore)]
        public int? SequenceNumber { get; set; }

        [JsonProperty("t", NullValueHandling = NullValueHandling.Ignore)]
        public string EventName { get; set; }
    }
}
