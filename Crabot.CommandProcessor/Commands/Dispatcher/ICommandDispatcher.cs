using System.Threading.Tasks;
using Crabot.Core.Repositories;

namespace Crabot.Commands.Dispatcher
{
    public interface ICommandDispatcher
    {
        Task DispatchAsync(Command command);
        Task DispatchAsync(Reaction reaction, TrackedMessage trackedMessage);
    }
}
