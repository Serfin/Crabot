using System;
using System.Threading.Tasks;
using Crabot.Commands.Commands;
using Crabot.Commands.Commands.Models;
using Crabot.Commands.Dispatcher;
using Crabot.Rest.Models;
using Crabot.Rest.RestClient;

namespace Crabot.Commands.Handlers
{
    [Command("ping")]
    public class PingCommandHandler : ICommandHandler
    {
        private readonly IDiscordRestClient _discordRestClient;

        public PingCommandHandler(IDiscordRestClient discordRestClient)
        {
            _discordRestClient = discordRestClient;
        }

        public async Task HandleAsync(Command command)
        {
            var response = await _discordRestClient.PostMessage(command.CalledFromChannel, 
                new Message { Content = "Calculating..." });

            var latency = DateTime.Now.Subtract(response.Data.Timestamp);

            await _discordRestClient.EditMessage(command.CalledFromChannel,
                response.Data.Id, new Message { Content = $"{latency.Milliseconds}ms" });
        }
    }
}
