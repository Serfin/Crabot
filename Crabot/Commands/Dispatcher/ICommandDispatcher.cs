using System.Threading.Tasks;
using Crabot.Contracts;

namespace Crabot.Commands.Dispatcher
{
    public interface ICommandDispatcher
    {
        Task DispatchAsync(Message command);
    }
}
