using System;
using System.Threading;
using System.Threading.Tasks;

namespace Crabot.WebSocket
{
    public interface IDiscordSocketClient
    {
        Task ConnectAsync(Uri address);
        Task DisconnectAsync();
        Task RunAsync(CancellationToken cancellationToken);
        Task SendAsync(byte[] payload, bool isEoF);
        event Func<string, Task> MessageReceive;
    }
}