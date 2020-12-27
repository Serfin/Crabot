using System.Linq;
using System.Threading.Tasks;
using Crabot.Commands.Dispatcher;
using Crabot.Commands.Models;
using Crabot.Repositories;
using Crabot.Rest.RestClient;

namespace Crabot.Commands.Handlers
{
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
                $"Command not found! <:{sadChamp.Name}:{sadChamp.Id}>");
        }
    }
}
