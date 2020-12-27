using System;
using System.Threading;
using System.Threading.Tasks;

namespace Crabot.Gateway.SocketClient
{
    public interface IDiscordSocketClient
    {
        int? SequenceNumber { get; set; }
        Task ConnectAsync(string address);
        Task RunAsync(CancellationToken cancellationToken);
        void RequestCancellation();
        Task SendAsync(byte[] payload, bool isEoF);
        event Func<GatewayPayload, Task> TextMessage;
    }
}