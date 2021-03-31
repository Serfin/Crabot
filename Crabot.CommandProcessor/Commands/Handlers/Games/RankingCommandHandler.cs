using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crabot.Commands.Dispatcher;
using Crabot.Core.Repositories;
using Crabot.MessageExtensions;
using Crabot.Rest.Models;
using Crabot.Rest.RestClient;

namespace Crabot.Commands.Commands.Handlers.Games
{
    [Command("ranking")]
    public class RankingCommandHandler : ICommandHandler
    {
        private readonly IUserPointsRepository _userPointsRepository;
        private readonly IGuildRepository _guildRepository;
        private readonly IDiscordRestClient _discordRestClient;

        public RankingCommandHandler(
            IUserPointsRepository userPointsRepository, 
            IDiscordRestClient discordRestClient,
            IGuildRepository guildRepository)
        {
            _userPointsRepository = userPointsRepository;
            _discordRestClient = discordRestClient;
            _guildRepository = guildRepository;
        }

        public async Task HandleAsync(Command command)
        {
            var users = await _userPointsRepository.GetUsersAsync();
            var embedMessage = new EbmedMessageBuilder()
                    .AddAuthor();

            foreach (var user in users.OrderByDescending(x => x.Balance).Take(15))
            {
                embedMessage.AddMessageField(new EmbedField($"<@{user.UserId}>", user.Balance.ToString()));
            }

            embedMessage.AddMessageFooter(DateTime.Now.ToShortDateString());

            await _discordRestClient.PostMessage(command.CalledFromChannel, 
                new Message { Embed = embedMessage.Build() });
        }
    }
}
