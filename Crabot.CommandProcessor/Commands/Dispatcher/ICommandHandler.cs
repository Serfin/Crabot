using System.Threading.Tasks;

namespace Crabot.Commands.Dispatcher
{
    public interface ICommandHandler<T>
        where T : ICommand
    {
        Task HandleAsync(T command);
    }
}
