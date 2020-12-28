using System.Threading.Tasks;
using Crabot.Core.Events;

namespace Crabot.Gateway
{
    public interface IGatewayEventDispatcher
    {
        Task DispatchEvent(GatewayPayload @event);
    }
}
