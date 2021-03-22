using Crabot.Contracts;

namespace Crabot.Commands.Commands
{
    public interface ICommandValidator
    {
        public bool IsCommand(Message message);
    }
}
