using Newtonsoft.Json;

namespace Crabot.Contracts
{
    public class ClientInfo
    {
        [JsonProperty("v")]
        public int V { get; set; }

        [JsonProperty("session_id")]
        public string SessionId { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }
    }

    public class User
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
