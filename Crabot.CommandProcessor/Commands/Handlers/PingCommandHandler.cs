using System;
using System.Threading.Tasks;
using Crabot.Commands.Commands;
using Crabot.Commands.Dispatcher;
using Crabot.Rest.Models;
using Crabot.Rest.RestClient;

namespace Crabot.Commands.Handlers
{
    [Command("ping", 0)]
    [CommandUsage("?ping")]
    public class PingCommandHandler : ICommandHandler
    {
        private readonly IDiscordRestClient _discordRestClient;

        public PingCommandHandler(IDiscordRestClient discordRestClient)
        {
            _discordRestClient = discordRestClient;
        }

        public async Task HandleAsync(Command command)
        {
            var requestSendTime = DateTime.Now;
            var response = await _discordRestClient.PostMessage(command.CalledFromChannel, 
                new Message { Content = "Calculating..." });

            var latency = response.Data.Timestamp - requestSendTime;

            await _discordRestClient.EditMessage(command.CalledFromChannel,
                response.Data.Id, new Message { Content = $"{latency.Milliseconds}ms" });
        }

        public async Task<ValidationResult> ValidateCommandAsync(Command command)
        {
            return await Task.FromResult(new ValidationResult(true));
        }
    }
}
