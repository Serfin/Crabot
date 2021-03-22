using System.Threading.Tasks;
using Crabot.Commands.Commands.Models;

namespace Crabot.Commands.Dispatcher
{
    public interface ICommandHandler
    {
        Task HandleAsync(Command command);
    }
}
