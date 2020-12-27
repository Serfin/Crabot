using System;
using System.Threading;
using System.Threading.Tasks;

namespace Crabot.Gateway.SocketClient
{
    public interface IDiscordSocketClient
    {
        Task ConnectAsync(string address);
        Task ReconnectAsync();
        Task StartListeningAsync(CancellationToken cancellationToken);
        Task SendAsync(byte[] payload, bool isEoF);
        event Func<GatewayPayload, Task> MessageReceive;
    }
}