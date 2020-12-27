using Newtonsoft.Json;

namespace Crabot
{
    public class HeartbeatEvent : IGatewayEvent
    {
        [JsonProperty("heartbeat_interval")]
        public int HeartbeatInterval { get; set; }
    }
}
