using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Crabot.Contracts
{
    public class Member
    {
        [JsonProperty("roles")]
        public List<string> Roles { get; set; }

        [JsonProperty("premium_since")]
        public string PremiumSince { get; set; }

        [JsonProperty("nick")]
        public string Nick { get; set; }

        [JsonProperty("mute")]
        public bool Mute { get; set; }

        [JsonProperty("joined_at")]
        public string JoinedAt { get; set; }

        [JsonProperty("is_pending")]
        public bool IsPending { get; set; }

        [JsonProperty("hoisted_role")]
        public string HoistedRole { get; set; }

        [JsonProperty("deaf")]
        public bool Deaf { get; set; }
    }
}
