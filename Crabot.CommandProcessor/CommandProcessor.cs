using System.Threading.Tasks;
using Crabot.Commands.Commands;
using Crabot.Commands.Dispatcher;
using Crabot.Contracts;
using Crabot.Core.Events;
using Crabot.Core.Repositories;
using Newtonsoft.Json;

namespace Crabot.Commands
{
    public class CommandProcessor : ICommandProcessor
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ICommandValidator _commandValidator;
        private readonly ITrackedMessageRepository _trackedMessageRepository;

        public CommandProcessor(
            ICommandDispatcher commandDispatcher,
            ICommandValidator commandValidator,
            ITrackedMessageRepository trackedMessageRepository)
        {
            _commandDispatcher = commandDispatcher;
            _commandValidator = commandValidator;
            _trackedMessageRepository = trackedMessageRepository;
        }

        public async Task ProcessMessageAsync(GatewayPayload payload)
        {
            var message = JsonConvert.DeserializeObject<GatewayMessage>(payload.EventData.ToString());

            if (_commandValidator.IsCommand(message))
            {
                var command = new Command(message);

                await _commandDispatcher.DispatchAsync(command);
            }
        }

        public async Task ProcessReactionAsync(GatewayPayload reaction)
        {
            if (reaction.EventName == "MESSAGE_REACTION_ADD")
            {
                var reactionAdd = JsonConvert.DeserializeObject<MessageReactionAdd>(
                    reaction.EventData.ToString());

                // Bot sometimes adds reactions to help user interaction so we skip handling
                if (!_commandValidator.IsBotAnAuthor(reactionAdd.UserId))
                {
                    var trackedMessage = await _trackedMessageRepository
                        .GetTrackedMessageAsync(reactionAdd.MessageId);

                    if (trackedMessage is null)
                    {
                        return;
                    }

                    await _commandDispatcher.DispatchAsync(new Reaction(reactionAdd), trackedMessage);
                }
            }
            else // MESSAGE_REACTION_REMOVE
            {
                var reactionRemove = JsonConvert.DeserializeObject<MessageReactionRemove>(
                    reaction.EventData.ToString());

                if (!_commandValidator.IsBotAnAuthor(reactionRemove.UserId))
                {
                    var trackedMessage = await _trackedMessageRepository
                        .GetTrackedMessageAsync(reactionRemove.MessageId);

                    if (trackedMessage is null)
                    {
                        return;
                    }

                    await _commandDispatcher.DispatchAsync(new Reaction(reactionRemove), trackedMessage);
                }
            }
        }
    }
}
