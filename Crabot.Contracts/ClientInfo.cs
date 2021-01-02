using System.Collections.Generic;
using Newtonsoft.Json;

namespace Crabot.Contracts
{
    public class ClientInfo
    {
        public int V { get; set; }
        [JsonProperty("session_id")]
        public string SessionId { get; set; }
        public List<GuildShort> Guilds { get; set; }
        public User User { get; set; }
    }

    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Avatar { get; set; }
    }

    public class GuildShort
    {
        public string Id { get; set; }
        public bool Unavailable { get; set; }
    }
}
