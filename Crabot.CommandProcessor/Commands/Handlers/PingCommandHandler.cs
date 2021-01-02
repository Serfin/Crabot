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
            await _discordRestClient.PostMessage(command.Message.ChannelId, 
                new Message { Content = "pong" });
        }
    }
}
