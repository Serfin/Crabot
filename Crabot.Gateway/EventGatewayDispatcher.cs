using System.Threading.Tasks;
using Autofac;
using Crabot.Commands;
using Crabot.Contracts;
using Crabot.Core.Events;
using Crabot.Core.Events.Voice;
using Crabot.WebSocket;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Crabot.Gateway
{
    public class EventGatewayDispatcher : IGatewayDispatcher
    {
        private readonly ILogger _logger;
        private readonly ICommandProcessor _commandProcessor;
        private readonly IComponentContext _componentContext;

        public EventGatewayDispatcher(
            ILogger<EventGatewayDispatcher> logger, 
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
                case "MESSAGE_DELETE":
                    {
                        // Delete tracked message
                    }
                    break;
                case "MESSAGE_REACTION_ADD":
                    {
                        await _commandProcessor.ProcessReactionAsync(@event);
                    }
                    break;
                case "MESSAGE_REACTION_REMOVE":
                    {
                        await _commandProcessor.ProcessReactionAsync(@event);
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
                case "VOICE_SERVER_UPDATE":
                    {
                        var parsed = JsonConvert.DeserializeObject<VoiceServerUpdate>(@event.EventData.ToString());
                        _componentContext.Resolve<VoiceGatewayConnection>().SetVoiceServerUpdate(parsed);
                    }
                    break;
                case "VOICE_STATE_UPDATE":
                    {
                        var parsed = JsonConvert.DeserializeObject<VoiceStateUpdate>(@event.EventData.ToString());
                        _componentContext.Resolve<VoiceGatewayConnection>().SetVoiceStateUpdate(parsed);

                        await _componentContext.ResolveNamed<IGatewayConnection>("voice-gateway-connection")
                            .StartConnectionAsync();
                    }
                    break;
                default:
                    _logger.LogWarning($"Unhandled event received [{@event.Opcode} - {@event.EventName}]");
                    break;
            }
        }
    }
}
