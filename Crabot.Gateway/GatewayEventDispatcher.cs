using System.Threading.Tasks;
using Autofac;
using Crabot.Commands;
using Crabot.Contracts;
using Crabot.Core.Events;
using Microsoft.Extensions.Logging;

namespace Crabot.Gateway
{
    public class GatewayEventDispatcher : IGatewayEventDispatcher
    {
        private readonly ILogger _logger;
        private readonly ICommandProcessor _commandProcessor;
        private readonly IComponentContext _componentContext;

        public GatewayEventDispatcher(
            ILogger<GatewayEventDispatcher> logger, 
            ICommandProcessor commandProcessor,
            IComponentContext componentContext)
        {
            _logger = logger;
            _commandProcessor = commandProcessor;
            _componentContext = componentContext;
        }

        public async Task DispatchEvent(GatewayPayload @event)
        {
            switch (@event.EventName)
            {
                case "MESSAGE_CREATE":
                    {
                        await _commandProcessor.ProcessMessageAsync(@event);
                    }
                    break;
                case "MESSAGE_REACTION_ADD":
                    {
                        await _componentContext.Resolve<IGatewayEventHandler<MessageReactionAdd>>()
                                .HandleAsync(@event.EventData);
                    }
                    break;
                case "MESSAGE_REACTION_REMOVE":
                    {
                        await _componentContext.Resolve<IGatewayEventHandler<MessageReactionRemove>>()
                                .HandleAsync(@event.EventData);
                    }
                    break;
                case "GUILD_CREATE":
                    {
                        await _componentContext.Resolve<IGatewayEventHandler<Guild>>()
                            .HandleAsync(@event.EventData);
                    }
                    break;
                case "READY":
                    {
                        await _componentContext.Resolve<IGatewayEventHandler<ReadyEvent>>()
                            .HandleAsync(@event.EventData);
                    }
                    break;
                case "RESUMED":
                    {
                        _logger.LogWarning("Session resumed!");
                    }
                    break;
                default:
                    _logger.LogWarning($"Unhandled event received [{@event.Opcode} - {@event.EventName}]");
                    break;
            }
        }
    }
}
