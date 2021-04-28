using System.Threading.Tasks;
using Crabot.Commands.Commands;

namespace Crabot.Commands.Dispatcher
{
    public interface ICommandHandler
    {
        Task<ValidationResult> ValidateCommandAsync(Command command);
        Task HandleAsync(Command command);
    }
}
