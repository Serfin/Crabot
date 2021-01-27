﻿using System.Linq;
using System.Threading.Tasks;
using Crabot.Commands.Commands;
using Crabot.Commands.Dispatcher;
using Crabot.Commands.Models;
using Crabot.Core.Repositories;
using Crabot.Rest.Models;
using Crabot.Rest.RestClient;

namespace Crabot.Commands.Handlers
{
    [Command("")]
    public class CommandErrorHandler : ICommandHandler<CommandError>
    {
        private readonly IDiscordRestClient _discordRestClient;
        private readonly IGuildRepository _guildRepository;

        public CommandErrorHandler(IDiscordRestClient discordRestClient, 
            IGuildRepository guildRepository)
        {
            _discordRestClient = discordRestClient;
            _guildRepository = guildRepository;
        }

        public async Task HandleAsync(CommandError command)
        {
            var sadChamp = _guildRepository.GetGuild(command.Message.GuildId)?
                .Emojis.FirstOrDefault(x => x.Name == "SadChamp");

            await _discordRestClient.PostMessage(command.Message.ChannelId,
                new Message { Content = $"Command not found! <:{sadChamp.Name}:{sadChamp.Id}>" });
        }
    }
}
