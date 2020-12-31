
using System;
using System.Threading.Tasks;
using Crabot.Commands;
using Crabot.Contracts;
using Crabot.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Crabot.Gateway
{
    public class GatewayEventDispatcher : IGatewayEventDispatcher
    {
        private readonly ILogger _logger;
        private readonly ICommandProcessor _commandProcessor;
        private readonly IServiceProvider _serviceProvider;

        public GatewayEventDispatcher(
            ILogger<GatewayEventDispatcher> logger, 
            ICommandProcessor commandProcessor,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _commandProcessor = commandProcessor;
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchEvent(GatewayPayload @event)
        {
            switch (@event.Opcode)
            {
                case GatewayOpCode.Dispatch:
                    if (@event.EventName == "MESSAGE_CREATE")
                    {
                        await _commandProcessor.ProcessMessageAsync(@event);
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
                        _logger.LogWarning($"Unhandled event received [{@event.Opcode} - {@event.EventName}]");
                    }
                    break;
                default:
                    _logger.LogWarning($"Unhandled event received [{@event.Opcode} - {@event.EventName}]");
                    break;
            }
        }
    }
}
