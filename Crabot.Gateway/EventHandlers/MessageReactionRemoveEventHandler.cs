using System.Threading.Tasks;
using Crabot.Core.Events;
using Crabot.Core.Repositories;
using Crabot.Rest.RestClient;
using Newtonsoft.Json;

namespace Crabot.Gateway.EventHandlers
{
    public class MessageReactionRemoveEventHandler : IGatewayEventHandler<MessageReactionRemove>
    {
        private readonly IDiscordRestClient _discordRestClient;
        private readonly IGuildRepository _guildRepository;

        public MessageReactionRemoveEventHandler(IDiscordRestClient discordRestClient,
            IGuildRepository guildRepository)
        {
            _discordRestClient = discordRestClient;
            _guildRepository = guildRepository;
        }

        public async Task HandleAsync(object @event)
        {
            var eventData = JsonConvert.DeserializeObject<MessageReactionRemove>(@event.ToString());

            await _discordRestClient.PostMessage("764840399696822322",
                new Rest.Models.Message
                {
                    Content = $"User <@{eventData.UserId}> removed his <:{eventData.Emoji.Name}:{eventData.Emoji.Id}> " +
                              $"reaction on message id: {eventData.MessageId}"
                });
        }
    }
}
