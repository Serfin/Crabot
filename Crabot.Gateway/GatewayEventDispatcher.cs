﻿
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
        static bool debugLoggingShort = true;
        static bool debugLoggingFull = false;

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
            if (debugLoggingShort)
            {
                if (!string.IsNullOrEmpty(@event.EventName))
                {
                    _logger.LogInformation($"[{@event.Opcode} - {@event.EventName}]");
                }
                else
                {
                    _logger.LogInformation($"[{@event.Opcode}]");
                }
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
                case GatewayOpCode.InvalidSession:
                    _logger.LogWarning("Cannot resume session!");
                    break;
                case GatewayOpCode.HeartbeatAck:
                    _logger.LogInformation("Session prolongate successful!");
                    break;
                default:
                    _logger.LogWarning($"Unhandled event received [{@event.Opcode} - {@event.EventName}]");
                    break;
            }
        }
    }
}