
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
        private readonly ILogger _logger;
        private readonly ICommandProcessor _commandProcessor;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDiscordRestClient _discordRestClient;

        public GatewayEventDispatcher(
            ILogger<GatewayEventDispatcher> logger, 
            ICommandProcessor commandProcessor, 
            IServiceProvider serviceProvider, 
            IDiscordRestClient discordRestClient)
        {
            _logger = logger;
            _commandProcessor = commandProcessor;
            _serviceProvider = serviceProvider;
            _discordRestClient = discordRestClient;
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
                case "GUILD_CREATE":
                    {
                        await _serviceProvider.GetRequiredService<IGatewayEventHandler<Guild>>()
                            .HandleAsync(@event.EventData);
                    }
                    break;
                case "READY":
                    {
                        await _serviceProvider.GetRequiredService<IGatewayEventHandler<ReadyEvent>>()
                            .HandleAsync(@event.EventData);
                    }
                    break;
                case "RESUMED":
                    {
                        await _discordRestClient.PostMessage("764840399696822322",
                            new Rest.Models.Message { Content = "```\nServer sent [Dispatch - RESUMED]\n```" });
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
