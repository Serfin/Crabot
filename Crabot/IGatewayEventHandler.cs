using System.Threading.Tasks;

namespace Crabot
{
    public interface IGatewayEventHandler<T>
    {
        Task HandleAsync(object @event);
    }
}
