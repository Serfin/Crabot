using System.Collections.Generic;

namespace Crabot.Core.Repositories
{
    public class TrackedMessage
    {
        public string GuildId { get; private set; }
        public string ChannelId { get; private set; }
        public string MessageId { get; private set; }
        public string Command { get; private set; }
        public IReadOnlyDictionary<string, string> ValidEmojis { get; private set; }

        public TrackedMessage(string guildId, string channelId, string messageId,
            string command, IReadOnlyDictionary<string, string> validEmojis)
        {
            GuildId = guildId;
            ChannelId = channelId;
            MessageId = messageId;
            Command = command;
            ValidEmojis = validEmojis;
        }
    }
}