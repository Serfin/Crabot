using System.Linq;
using System.Threading.Tasks;
using Crabot.Commands.Commands;
using Crabot.Commands.Dispatcher;
using Crabot.Core.Repositories;
using Crabot.Rest.RestClient;

namespace Crabot.Commands.Handlers
{
    [Command("internal-application-error", 0)]
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
                $"Internal application error <:{sadChamp.Name}:{sadChamp.Id}>");
        }

        public Task<ValidationResult> ValidateCommandAsync(Command command)
        {
            return Task.FromResult(new ValidationResult(true));
        }
    }
}
