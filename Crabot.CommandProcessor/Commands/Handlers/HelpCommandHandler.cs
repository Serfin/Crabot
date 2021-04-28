using System.Threading.Tasks;
using Crabot.Commands.Dispatcher;
using Crabot.MessageExtensions;
using Crabot.Rest.Models;
using Crabot.Rest.RestClient;

namespace Crabot.Commands.Commands.Handlers
{
    [Command("help", 0)]
    [CommandUsage("?help")]
    public class HelpCommandHandler : ICommandHandler
    {
        private readonly IDiscordRestClient _discordRestClient;

        public HelpCommandHandler(IDiscordRestClient discordRestClient)
        {
            _discordRestClient = discordRestClient;
        }

        public async Task HandleAsync(Command command)
        {
            var embedMessage = new EbmedMessageBuilder()
                .AddAuthor()
                .AddMessageBody("TEST_BODY")
                .AddMessageFields(
                    new EmbedField("Test", "test"),
                    new EmbedField("Test", "test"))
                .AddMessageFooter("TEST_FOOTER")
                .Build();

            await _discordRestClient.PostMessage(command.CalledFromChannel, 
                new Message { Embed = embedMessage });
        }

        public Task<ValidationResult> ValidateCommandAsync(Command command)
        {
            return Task.FromResult(new ValidationResult(true));
        }
    }
}
