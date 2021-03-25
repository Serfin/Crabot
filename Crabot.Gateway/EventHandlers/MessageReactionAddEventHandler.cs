using System.Threading.Tasks;
using Crabot.Core.Events;
using Crabot.Core.Repositories;
using Crabot.Rest.RestClient;
using Newtonsoft.Json;

namespace Crabot.Gateway.EventHandlers
{
    public class MessageReactionAddEventHandler : IGatewayEventHandler<MessageReactionAdd>
    {
        private readonly IDiscordRestClient _discordRestClient;
        private readonly IGuildRepository _guildRepository;

        public MessageReactionAddEventHandler(IDiscordRestClient discordRestClient,
            IGuildRepository guildRepository)
        {
            _discordRestClient = discordRestClient;
            _guildRepository = guildRepository;
        }

        public async Task HandleAsync(object @event)
        {
            var eventData = JsonConvert.DeserializeObject<MessageReactionAdd>(@event.ToString());

            await _discordRestClient.PostMessage("764840399696822322",
                new Rest.Models.Message
                {
                    Content = $"Found <:{eventData.Emoji.Name}:{eventData.Emoji.Id}> reaction added by <@{eventData.Member.User.Id}>" +
                              $" on message id: {eventData.MessageId}"
                });
        }
    }
}
