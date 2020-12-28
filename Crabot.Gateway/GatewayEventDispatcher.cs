using System;
using System.Threading.Tasks;
using Crabot.Commands;
using Crabot.Contracts;
using Crabot.Core.Events;
using Crabot.Rest.RestClient;
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
        private readonly IDiscordRestClient _discordRestClient;
        private readonly IServiceProvider _serviceProvider;

        public GatewayEventDispatcher(
            ILogger<GatewayEventDispatcher> logger, 
            ICommandProcessor commandProcessor, 
            IDiscordRestClient discordRestClient,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _commandProcessor = commandProcessor;
            _discordRestClient = discordRestClient;
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
                        _logger.LogWarning($"Unhandled event received [{@event.Opcode} - {@event.EventName}] - [{@event.EventData}]");
                    }
                    break;
                case GatewayOpCode.Hello:
                    await _serviceProvider.GetRequiredService<IGatewayEventHandler<HeartbeatEvent>>()
                            .HandleAsync(@event.EventData);
                    await _discordRestClient.PostMessage("764840399696822322", "Heartbeat interval - " + @event.EventData.ToString());
                    break;
                case GatewayOpCode.Reconnect:
                    _logger.LogWarning("Server requesting reconnect");
                    await _discordRestClient.PostMessage("764840399696822322", "Server requesting reconnect");
                    await _serviceProvider.GetRequiredService<IGatewayEventHandler<ReconnectEvent>>()
                            .HandleAsync(@event.EventData);
                    await _discordRestClient.PostMessage("764840399696822322", "Successfully resumed session!");
                    break;
                case GatewayOpCode.InvalidSession:
                    _logger.LogWarning("Cannot resume session!");
                    await _discordRestClient.PostMessage("764840399696822322", "Cannot resume session!");
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
