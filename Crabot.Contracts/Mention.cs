﻿using Newtonsoft.Json;

namespace Crabot.Contracts
{
    public class Mention
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("public_flags")]
        public int PublicFlags { get; set; }

        [JsonProperty("member")]
        public Member Member { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("discriminator")]
        public string Discriminator { get; set; }

        [JsonProperty("avatar")]
        public object Avatar { get; set; }
    }
}
