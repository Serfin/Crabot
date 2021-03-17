using Crabot.Commands.Commands;
using Crabot.Commands.Dispatcher;
using Crabot.Contracts;

namespace Crabot.Commands.Models
{
    [Command("error")]
    public class CommandError : ICommand
    {
        public Message Message { get; private set; }

        public CommandError(Message message)
        {
            Message = message;
        }
    }
}
