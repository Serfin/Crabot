using System;
using System.Threading.Tasks;
using Crabot.Core.Events;

namespace Crabot.WebSocket
{
    public interface IGatewayConnection
    {
        public Task StartConnectionAsync();

        public event Func<GatewayPayload, Task> EventReceive;
    }
}
