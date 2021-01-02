using Newtonsoft.Json;

namespace Crabot.MessageExtensions
{
    public class EmbedAuthor
    {
        public string Name { get; set; }
        public string Url { get; set; }
        [JsonProperty("icon_url")]
        public string IconUrl { get; set; } = "https://cdn.discordapp.com/avatars/692462941337026673/bf4f5f4912a292ddab2b37157bccfe3f.png";
    }
}
