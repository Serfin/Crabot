using System.Threading.Tasks;

namespace Crabot.Commands.Dispatcher
{
    public interface IReactionHandler
    {
        Task HandleAsync(Reaction reaction);
    }
}


// { message_id: [list of reactions should handle]}