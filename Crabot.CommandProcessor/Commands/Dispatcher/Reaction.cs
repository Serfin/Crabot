using Crabot.Core.Events;
using Crabot.Core.Events.NestedTypes;

namespace Crabot.Commands.Dispatcher
{
    public class Reaction
    {
        public string GuildId { get; private set; }
        public string ChannelId { get; private set; }
        public string MessageId { get; private set; }
        public Emoji Emoji { get; private set; }
        public string UserId { get; private set; }
        public Member Member { get; private set; }
        public bool IsAdded { get; private set; }

        public Reaction(MessageReactionAdd reactionAdd)
        {
            GuildId = reactionAdd.GuildId;
            ChannelId = reactionAdd.ChannelId;
            MessageId = reactionAdd.MessageId;
            Emoji = reactionAdd.Emoji;
            UserId = reactionAdd.UserId;
            Member = reactionAdd.Member;
            IsAdded = true;
        }

        public Reaction(MessageReactionRemove reactionRemove)
        {
            GuildId = reactionRemove.GuildId;
            ChannelId = reactionRemove.ChannelId;
            MessageId = reactionRemove.MessageId;
            Emoji = reactionRemove.Emoji;
            UserId = reactionRemove.UserId;
            IsAdded = false;
        }
    }
}
