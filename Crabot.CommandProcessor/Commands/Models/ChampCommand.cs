using Crabot.Commands.Dispatcher;
using Crabot.Contracts;

namespace Crabot.Commands.Commands.Models
{
    public class ChampCommand : ICommand
    {
        public Message Message { get; private set; }

        public ChampCommand(Message message)
        {
            Message = message;
        }
    }
}
