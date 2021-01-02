using Newtonsoft.Json;

namespace Crabot.MessageExtensions
{
    public class EmbedFooter
    {
        [JsonProperty("icon_url")]
        public string IconUrl { get; set; }
        public string Text { get; set; }
    }
}
