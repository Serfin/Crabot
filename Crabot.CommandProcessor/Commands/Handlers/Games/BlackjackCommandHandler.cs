using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Crabot.Commands.Commands;
using Crabot.Commands.Dispatcher;
using Crabot.Core.Repositories;
using Crabot.Rest.Models;
using Crabot.Rest.RestClient;

namespace Crabot.Commands.Handlers.Games
{
    [Command("blackjack")]
    public class BlackjackCommandHandler : ICommandHandler, IReactionHandler 
    {
        private readonly IDiscordRestClient _discordRestClient;
        private readonly IUserPointsRepository _usersPointsRepository;
        private readonly ITrackedMessageRepository _trackedMessageRepository;

        public BlackjackCommandHandler(
            IDiscordRestClient discordRestClient, 
            IUserPointsRepository usersPointsRepository,
            ITrackedMessageRepository trackedMessageRepository)
        {
            _discordRestClient = discordRestClient;
            _usersPointsRepository = usersPointsRepository;
            _trackedMessageRepository = trackedMessageRepository;
        }

        public async Task HandleAsync(Command command)
        {
            var response = await _discordRestClient.PostMessage(command.CalledFromChannel,
                new Message { Content = "Test tracked message" });

            await _trackedMessageRepository.AddTrackedMessageAsync(
                new TrackedMessage(command.CalledFromGuild,
                command.CalledFromChannel, response.Data.Id,
                new Dictionary<string, string> { { "SadChamp", "778732414808883220" } }));
        }

        public Task HandleAsync(Reaction reaction)
        {
            throw new NotImplementedException();
        }
    }
}