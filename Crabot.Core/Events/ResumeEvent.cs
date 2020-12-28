﻿using Newtonsoft.Json;

namespace Crabot.Core.Events
{
    public class ResumeEvent : IGatewayEvent
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("session_id")]
        public string SessionId { get; set; }

        [JsonProperty("seq")]
        public int Sequence { get; set; }
    }
}
