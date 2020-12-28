using System.Threading.Tasks;

namespace Crabot.Gateway
{
    public interface IGatewayEventHandler<T>
    {
        Task HandleAsync(object @event);
    }
}
