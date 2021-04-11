using System.Collections.Generic;

namespace Crabot.Core.Repositories
{
    public class TrackedMessage
    {
        public string GuildId { get; private set; }
        public string ChannelId { get; private set; }
        public string MessageId { get; private set; }
        public IReadOnlyDictionary<string, string> ValidEmojis { get; private set; }

        public TrackedMessage(string guildId, string channelId, string messageId,
            IReadOnlyDictionary<string, string> validEmojis)
        {
            GuildId = guildId;
            ChannelId = channelId;
            MessageId = messageId;
            ValidEmojis = validEmojis;
        }
    }
}