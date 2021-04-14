using Crabot.Contracts;

namespace Crabot.Commands.Commands
{
    public interface ICommandValidator
    {
        public bool IsCommand(GatewayMessage message);
        public bool IsBotAnAuthor(string userId);
    }
}
