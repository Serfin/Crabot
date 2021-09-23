using Newtonsoft.Json;

namespace Crabot.Core.Events
{
    public class GatewayPayload
    {
        [JsonProperty("op")]
        public int Opcode { get; set; }

        [JsonProperty("d")]
        public object EventData { get; set; }

        [JsonProperty("s")]
        public int? SequenceNumber { get; set; }

        [JsonProperty("t")]
        public string EventName { get; set; }
    }
}
