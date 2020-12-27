using Crabot.Commands.Dispatcher;
using Crabot.Contracts;

namespace Crabot.Commands.Models
{
    public class PingCommand : ICommand
    {
        public Message Message { get; private set; }

        public PingCommand(Message message)
        {
            Message = message;
        }
    }
}
