using Crabot.Commands.Dispatcher;
using Crabot.Contracts;

namespace Crabot.Commands.Commands.Models
{
    [Command("monster")]
    public class MonsterCommand : ICommand
    {
        public Message Message { get; private set; }

        public MonsterCommand(Message message)
        {
            Message = message;
        }
    }
}
