﻿using System;
using System.Collections.Generic;
using Crabot.Contracts;

namespace Crabot.Commands.Dispatcher
{
    public class Command
    {
        public string CommandName { get; private set; }
        public IReadOnlyList<string> Arguments { get; private set; }
        public Member Member { get; private set; } // Per server account 
        public Author Author { get; private set; } // General Discord account
        public string CalledFromChannel { get; private set; }
        public string CalledFromGuild { get; private set; }

        public Command(GatewayMessage message)
        {
            Member = message.Member;
            Author = message.Author;
            Arguments = message.Content.Split(' ', StringSplitOptions.RemoveEmptyEntries)[1..];
            CommandName = message.Content.Split(' ')[0].ToLowerInvariant()[1..];
            CalledFromChannel = message.ChannelId;
            CalledFromGuild = message.GuildId;
        }
    }
}
