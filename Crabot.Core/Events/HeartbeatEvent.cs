using Newtonsoft.Json;

namespace Crabot.Core.Events
{
    public class HeartbeatEvent : IGatewayEvent
    {
        [JsonProperty("heartbeat_interval")]
        public int HeartbeatInterval { get; set; }
    }
}
