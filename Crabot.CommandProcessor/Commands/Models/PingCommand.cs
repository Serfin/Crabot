using Crabot.Commands.Commands;
using Crabot.Commands.Dispatcher;
using Crabot.Contracts;

namespace Crabot.Commands.Models
{
    [Command("ping")]
    public class PingCommand : ICommand
    {
        public Message Message { get; private set; }

        public PingCommand(Message message)
        {
            Message = message;
        }
    }
}
