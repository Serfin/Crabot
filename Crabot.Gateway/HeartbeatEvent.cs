using Newtonsoft.Json;

namespace Crabot.Gateway
{
    public class HeartbeatEvent : IGatewayEvent
    {
        [JsonProperty("heartbeat_interval")]
        public int HeartbeatInterval { get; set; }
    }
}
