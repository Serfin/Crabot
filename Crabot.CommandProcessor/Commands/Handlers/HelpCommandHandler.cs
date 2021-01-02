using System.Threading.Tasks;
using Crabot.Commands.Commands.Models;
using Crabot.Commands.Dispatcher;
using Crabot.MessageExtensions;
using Crabot.Rest.Models;
using Crabot.Rest.RestClient;

namespace Crabot.Commands.Commands.Handlers
{
    public class HelpCommandHandler : ICommandHandler<HelpCommand>
    {
        private readonly IDiscordRestClient _discordRestClient;

        public HelpCommandHandler(IDiscordRestClient discordRestClient)
        {
            _discordRestClient = discordRestClient;
        }

        public async Task HandleAsync(HelpCommand command)
        {
            var embedMessage = new EbmedMessageBuilder()
                .AddAuthor()
                .AddMessageBody("TEST_BODY")
                .AddMessageFields(
                    new EmbedField("Test", "test"),
                    new EmbedField("Test", "test"))
                .AddMessageFooter("TEST_FOOTER")
                .Build();

            await _discordRestClient.PostMessage(command.Message.ChannelId, 
                new Message { Embed = embedMessage });
        }
    }
}
