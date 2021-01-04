using System;
using System.Threading.Tasks;
using Crabot.Core.Events;

namespace Crabot.WebSocket
{
    public interface IConnectionManager
    {
        public void SetSequenceNumber(int? sequenceNumber);
        public Task CreateConnectionAsync();
        public Task RunHeartbeat(int heartbeatInterval);

        public event Func<GatewayPayload, Task> EventReceive;
    }
}
