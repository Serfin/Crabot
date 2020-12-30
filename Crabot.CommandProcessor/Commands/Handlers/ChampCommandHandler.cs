using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crabot.Commands.Commands.Models;
using Crabot.Commands.Dispatcher;
using Crabot.Core.Repositories;
using Crabot.Rest.RestClient;

namespace Crabot.Commands.Commands.Handlers
{
    public class ChampCommandHandler : ICommandHandler<ChampCommand>
    {
        private readonly IDiscordRestClient _discordRestClient;
        private readonly IGuildRepository _guildRepository;

        public ChampCommandHandler(IDiscordRestClient discordRestClient, 
            IGuildRepository guildRepository)
        {
            _discordRestClient = discordRestClient;
            _guildRepository = guildRepository;
        }

        public async Task HandleAsync(ChampCommand command)
        {
            var guildEmotes = _guildRepository.GetGuild(command.Message.GuildId)?.Emojis;

            if (guildEmotes != null)
            {
                var stringBuilder = new StringBuilder();
                var champEmotes = guildEmotes.Where(x => x.Name.Contains("champ", 
                    StringComparison.InvariantCultureIgnoreCase));

                foreach (var emote in champEmotes)
                {
                    stringBuilder.AppendFormat("<:{0}:{1}>", emote.Name,
                        emote.Id);
                }

                await _discordRestClient.PostMessage(command.Message.ChannelId, stringBuilder.ToString());
            }
        }
    }
}
