using System.Threading.Tasks;
using Crabot.Commands.Dispatcher;
using Crabot.Core.Repositories;
using Crabot.Rest.RestClient;

namespace Crabot.Commands.Commands.Handlers.Games
{
    [Command("register")]
    public class RegisterCommandHandler : ICommandHandler
    {
        private readonly IUserPointsRepository _userPointsRepository;
        private readonly IDiscordRestClient _discordRestClient;

        public RegisterCommandHandler(
            IUserPointsRepository userPointsRepository, 
            IDiscordRestClient discordRestClient)
        {
            _userPointsRepository = userPointsRepository;
            _discordRestClient = discordRestClient;
        }

        public async Task HandleAsync(Command command)
        {
            var userBalance = await _userPointsRepository.GetUserBalanceAsync(command.Author.Id);

            if (userBalance is null)
            {
                await _userPointsRepository.AddUserToSystem(command.Author.Username,
                    command.Author.Id);

                await _discordRestClient.PostMessage(command.CalledFromChannel,
                   new Rest.Models.Message { Content = "Wallet created" });
            }
            else
            {
                await _discordRestClient.PostMessage(command.CalledFromChannel,
                    new Rest.Models.Message { Content = "You alread own a wallet" });
            }
        }
    }
}
