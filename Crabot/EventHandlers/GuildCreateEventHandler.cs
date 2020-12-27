using System.Threading.Tasks;
using Crabot.Contracts;
using Crabot.Core.Repositories;
using Newtonsoft.Json;

namespace Crabot
{
    public class GuildCreateEventHandler : IGatewayEventHandler<Guild>
    {
        private readonly IGuildRepository _guildRepository;

        public GuildCreateEventHandler(IGuildRepository guildRepository)
        {
            _guildRepository = guildRepository;
        }

        public async Task HandleAsync(object @event)
        {
            _guildRepository.AddGuild(JsonConvert.DeserializeObject<Guild>(@event.ToString()));

            await Task.CompletedTask;
        }
    }
}
