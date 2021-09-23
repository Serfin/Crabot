using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Crabot.Core.Events.Voice
{
    public class VoiceIdentify
    {
        [JsonProperty("server_id")]
        public string ServerId { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("session_id")]
        public string SessionId { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
