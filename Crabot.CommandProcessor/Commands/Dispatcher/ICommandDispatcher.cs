using System.Threading.Tasks;

namespace Crabot.Commands.Dispatcher
{
    public interface ICommandDispatcher
    {
        Task DispatchAsync(Command command);
        Task DispatchAsync(Reaction reaction);
    }
}
