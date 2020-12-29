using System.Threading.Tasks;
using Crabot.Contracts;
using Crabot.Core.Repositories;
using Newtonsoft.Json;

namespace Crabot.Gateway.EventHandlers
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
            var guild = JsonConvert.DeserializeObject<Guild>(@event.ToString());

            if (_guildRepository.GetGuild(guild.Id) != null)
            {
                _guildRepository.DeleteGuild(guild.Id);
            }

            _guildRepository.AddGuild(JsonConvert.DeserializeObject<Guild>(@event.ToString()));

            await Task.CompletedTask;
        }
    }
}
