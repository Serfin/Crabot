using System.Threading.Tasks;
using Crabot.Core.Events;

namespace Crabot.Gateway
{
    public interface IGatewayDispatcher
    {
        Task DispatchEvent(GatewayPayload @event);
    }
}
