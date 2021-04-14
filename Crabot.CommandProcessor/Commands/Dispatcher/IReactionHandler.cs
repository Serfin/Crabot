using System.Threading.Tasks;

namespace Crabot.Commands.Dispatcher
{
    public interface IReactionHandler
    {
        Task HandleAsync(Reaction reaction);
    }
}