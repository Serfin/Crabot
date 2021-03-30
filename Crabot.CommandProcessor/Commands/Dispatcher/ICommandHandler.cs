using System.Threading.Tasks;

namespace Crabot.Commands.Dispatcher
{
    public interface ICommandHandler
    {
        Task HandleAsync(Command command);
    }
}
