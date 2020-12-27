using System;
using System.Threading.Tasks;
using Crabot.Contracts;
using Crabot.Gateway;
using Crabot.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Crabot
{
    public class EventDispatcher : IEventDispatcher
    {
        static bool debugLoggingShort = true;
        static bool debugLoggingFull = false;

        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public EventDispatcher(
            ILogger<EventDispatcher> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchEvent(GatewayPayload @event)
        {
            if (debugLoggingShort)
            {
                _logger.LogInformation($"[{@event.Opcode} - {@event.EventName}]");
            }

            if (debugLoggingFull)
            {
                _logger.LogInformation($"[{@event.Opcode} - {@event.EventName}] \n [{@event.EventData}]");
            }

            switch (@event.Opcode)
            {
                case GatewayOpCode.Dispatch:
                    if (@event.EventName == "MESSAGE_CREATE")
                    {
                        await _serviceProvider.GetRequiredService<IGatewayEventHandler<MessageCreatedEvent>>()
                            .HandleAsync(@event.EventData);
                    }
                    else if (@event.EventName == "GUILD_CREATE")
                    {
                        await _serviceProvider.GetRequiredService<IGatewayEventHandler<Guild>>()
                            .HandleAsync(@event.EventData);
                    }
                    else if (@event.EventName == "READY")
                    {
                        await _serviceProvider.GetRequiredService<IGatewayEventHandler<ReadyEvent>>()
                            .HandleAsync(@event.EventData);
                    }
                    else
                    {
                        _logger.LogWarning($"Unhandled event received [{@event.Opcode} - {@event.EventName}] - [{@event.EventData}]");
                    }

                    break;
                case GatewayOpCode.Hello:
                    await _serviceProvider.GetRequiredService<IGatewayEventHandler<IdentifyEvent>>()
                        .HandleAsync(@event.EventData);
                    await _serviceProvider.GetRequiredService<IGatewayEventHandler<HeartbeatEvent>>()
                        .HandleAsync(@event);
                    break;
                case GatewayOpCode.Reconnect:
                    await _serviceProvider.GetRequiredService<IGatewayEventHandler<ResumeEvent>>()
                        .HandleAsync(@event);
                    _logger.LogInformation($"[{@event.Opcode} - {@event.EventName}] \n [{@event.EventData}]");
                    break;
                case GatewayOpCode.InvalidSession:
                    _logger.LogWarning("Cannot resume session! Starting new session!");
                    await _serviceProvider.GetRequiredService<IGatewayEventHandler<IdentifyEvent>>()
                        .HandleAsync(@event.EventData);
                    _logger.LogInformation("New session started!");
                    break;
                case GatewayOpCode.Resume:
                    _logger.LogInformation("Existing session resumed!");
                    break;
                case GatewayOpCode.HeartbeatAck:
                    _logger.LogInformation("Session prolongate successful!");
                    break;
                default:
                    _logger.LogWarning($"Unhandled event received [{@event.Opcode} - {@event.EventName}] - [{@event.EventData}]");
                    break;
            }
        }
    }
}
