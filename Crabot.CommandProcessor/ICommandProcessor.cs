using System.Threading.Tasks;
using Crabot.Core.Events;

namespace Crabot.Commands
{
    public interface ICommandProcessor
    {
        Task ProcessMessageAsync(GatewayPayload message);
        Task ProcessReactionAsync(GatewayPayload reaction);
    }
}
