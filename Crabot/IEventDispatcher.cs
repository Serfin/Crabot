using System.Threading.Tasks;
using Crabot.Gateway;

namespace Crabot
{
    public interface IEventDispatcher
    {
        Task DispatchEvent(GatewayPayload @event);
    }
}
