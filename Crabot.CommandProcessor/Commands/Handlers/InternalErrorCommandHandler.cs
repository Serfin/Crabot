using System.Linq;
using System.Threading.Tasks;
using Crabot.Commands.Commands;
using Crabot.Commands.Dispatcher;
using Crabot.Core.Repositories;
using Crabot.Rest.Models;
using Crabot.Rest.RestClient;

namespace Crabot.Commands.Handlers
{
    [Command("internal-application-error")]
    public class InternalErrorCommandHandler : ICommandHandler
    {
        private readonly IDiscordRestClient _discordRestClient;
        private readonly IGuildRepository _guildRepository;

        public InternalErrorCommandHandler(IDiscordRestClient discordRestClient,
            IGuildRepository guildRepository)
        {
            _discordRestClient = discordRestClient;
            _guildRepository = guildRepository;
        }

        public async Task HandleAsync(Command command)
        {
            var sadChamp = _guildRepository.GetGuild(command.CalledFromGuild)?
                .Emojis.FirstOrDefault(x => x.Name == "SadChamp");

            await _discordRestClient.PostMessage(command.CalledFromChannel,
                new Message { Content = $"Internal application error <:{sadChamp.Name}:{sadChamp.Id}>" });
        }
    }
}
