using System;
using System.Threading.Tasks;
using Crabot.Commands.Dispatcher;
using Crabot.Commands.Models;
using Crabot.Rest.Models;
using Crabot.Rest.RestClient;

namespace Crabot.Commands.Handlers
{
    public class PingCommandHandler : ICommandHandler<PingCommand>
    {
        private readonly IDiscordRestClient _discordRestClient;

        public PingCommandHandler(IDiscordRestClient discordRestClient)
        {
            _discordRestClient = discordRestClient;
        }

        public async Task HandleAsync(PingCommand command)
        {
            var response = await _discordRestClient.PostMessage(command.Message.ChannelId, 
                new Message { Content = "Calculating..." });

            var latency = DateTime.Now.Subtract(response.Data.Timestamp);

            await _discordRestClient.EditMessage(response.Data.ChannelId,
                response.Data.Id, new Message { Content = $"{latency.Milliseconds}ms" });
        }
    }
}
