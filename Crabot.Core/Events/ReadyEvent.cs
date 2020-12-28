using Newtonsoft.Json;

namespace Crabot.Core.Events
{
    public class ReadyEvent : IGatewayEvent
    {
        [JsonProperty("v")]
        public int V { get; set; }

        [JsonProperty("session_id")]
        public string SessionId { get; set; }

        [JsonProperty("user")]
        public User UserId { get; set; }
    }

    public class User
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
