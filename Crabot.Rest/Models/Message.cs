using Crabot.MessageExtensions;

namespace Crabot.Rest.Models
{
    public class Message
    {
        public string Content { get; set; }
        public EmbedMessage Embed { get; set; }
    }
}
