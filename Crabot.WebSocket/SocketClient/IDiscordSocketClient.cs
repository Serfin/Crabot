using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Crabot.WebSocket
{
    public interface IDiscordSocketClient
    {
        Task ConnectAsync(Uri address);
        Task DisconnectAsync(WebSocketCloseStatus socketCloseStatus);
        Task SendAsync(byte[] payload, bool isEoF);
        event Func<string, Task> MessageReceive;
    }
}