using System.Collections.Generic;
using Crabot.Contracts;

namespace Crabot.Commands.Commands.Models
{
    public class Command
    {
        public string CommandName { get; private set; }
        public IReadOnlyList<string> Arguments { get; private set; }
        public Member Caller { get; private set; }
        public string CalledFromChannel { get; private set; }
        public string CalledFromGuild { get; private set; }


        public Command(Message message)
        {
            Caller = message.Member;
            Arguments = message.Content.Split(' ');
            CommandName = message.Content.ToLower()[1..];
            CalledFromChannel = message.ChannelId;
            CalledFromGuild = message.GuildId;
        }
    }
}
