using System;
using System.Threading.Tasks;
using Crabot.Contracts;
using Crabot.Core;
using Crabot.Gateway;
using Crabot.Models;
using Crabot.Rest.RestClient;
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
        private readonly IConnectionManager _connectionManager;

        // Temp logging
        private readonly IDiscordRestClient _discordRestClient;

        public EventDispatcher(
            ILogger<EventDispatcher> logger,
            IServiceProvider serviceProvider,
            IConnectionManager connectionManager,
            IDiscordRestClient discordRestClient)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _connectionManager = connectionManager;
            _discordRestClient = discordRestClient;
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

            _connectionManager.SetSequenceNumber(@event.SequenceNumber);

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
                    await _connectionManager.CreateConnection(@event);
                    break;
                case GatewayOpCode.Reconnect:
                    await _discordRestClient.PostMessage("764840399696822322", "Server requesting reconnect - " + @event?.EventData?.ToString());
                    await _connectionManager.CreateConnection(@event);
                    break;
                case GatewayOpCode.InvalidSession:
                    await _discordRestClient.PostMessage("764840399696822322", "Invalid session!. Server requesting new session - " + @event?.EventData?.ToString());
                    _logger.LogWarning("Cannot resume session! Starting new session!");
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
