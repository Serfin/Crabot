using System.Threading.Tasks;
using Crabot.Commands.Dispatcher;
using Crabot.Core.Repositories;
using Crabot.Rest.RestClient;

namespace Crabot.Commands.Commands.Handlers.Games
{
    [Command("get-points")]
    public class GetPointsCommandHandler : ICommandHandler
    {
        private readonly IUserPointsRepository _userPointsRepository;
        private readonly IDiscordRestClient _discordRestClient;

        public GetPointsCommandHandler(
            IUserPointsRepository userPointsRepository, 
            IDiscordRestClient discordRestClient)
        {
            _userPointsRepository = userPointsRepository;
            _discordRestClient = discordRestClient;
        }

        public async Task HandleAsync(Command command)
        {
            if (await _userPointsRepository.CanUseDailyPoints(command.Author.Id))
            {
                await _userPointsRepository.AddBalanceToUserAccount(command.Author.Id, 1000);
                await _userPointsRepository.UpdateDailyPoints(command.Author.Id);
                await _discordRestClient.PostMessage(command.CalledFromChannel,
                    new Rest.Models.Message { Content = $"Added 1000 points to user **{command.Author.Username}**" });
            }
            else
            {
                await _discordRestClient.PostMessage(command.CalledFromChannel,
                    new Rest.Models.Message { Content = $"You have used your free points in last 10 minutes" });
            }
        }
    }
}
