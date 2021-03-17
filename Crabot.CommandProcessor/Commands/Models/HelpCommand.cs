using Crabot.Commands.Dispatcher;
using Crabot.Contracts;

namespace Crabot.Commands.Commands.Models
{
    [Command("help")]
    public class HelpCommand : ICommand
    {
        public Message Message { get; private set; }

        public HelpCommand(Message message)
        {
            Message = message;
        }
    }
}
