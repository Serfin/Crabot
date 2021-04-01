using System;
using System.Threading.Tasks;
using Crabot.Commands.Dispatcher;
using Crabot.Core.Repositories;
using Crabot.Rest.Models;
using Crabot.Rest.RestClient;

namespace Crabot.Commands.Commands.Handlers.Games
{
    [Command("wallet")]
    public class WalletCommandHandler : ICommandHandler
    {
        private readonly IUserPointsRepository _userPointsRepository;
        private readonly IDiscordRestClient _discordRestClient;

        public WalletCommandHandler(
            IUserPointsRepository userPointsRepository, 
            IDiscordRestClient discordRestClient)
        {
            _userPointsRepository = userPointsRepository;
            _discordRestClient = discordRestClient;
        }

        public async Task HandleAsync(Command command)
        {
            var userPoints = await _userPointsRepository.GetUserBalanceAsync(command.Author.Id);

            if (userPoints is null)
            {
                await _discordRestClient.PostMessage(command.CalledFromChannel,
                    new Message { Content = $"Can't find your wallet. Use ?register to create wallet" });

                return;
            }
                        
            await _discordRestClient.PostMessage(command.CalledFromChannel,
                new Message { Content = $"**{command.Author.Username}** currently has **{userPoints}** points" });
        }
    }
}
