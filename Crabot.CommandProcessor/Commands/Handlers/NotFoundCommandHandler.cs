using System.Linq;
using System.Threading.Tasks;
using Crabot.Commands.Commands;
using Crabot.Commands.Dispatcher;
using Crabot.Core.Repositories;
using Crabot.Rest.RestClient;

namespace Crabot.Commands.Handlers
{
    [Command("command-not-found", 0)]
    public class NotFoundCommandHandler : ICommandHandler
    {
        private readonly IDiscordRestClient _discordRestClient;
        private readonly IGuildRepository _guildRepository;

        public NotFoundCommandHandler(IDiscordRestClient discordRestClient, 
            IGuildRepository guildRepository)
        {
            _discordRestClient = discordRestClient;
            _guildRepository = guildRepository;
        }

        public async Task HandleAsync(Command command)
        {
            var huhChamp = _guildRepository.GetGuild(command.CalledFromGuild)?
                .Emojis.FirstOrDefault(x => x.Name == "HuhChamp");

            await _discordRestClient.PostMessage(command.CalledFromChannel,
                $"Invalid command <:{huhChamp.Name}:{huhChamp.Id}>");
        }

        public Task<ValidationResult> ValidateCommandAsync(Command command)
        {
            return Task.FromResult(new ValidationResult(true));
        }
    }
}
