using System.Threading.Tasks;
using Crabot.Core.Events;
using Crabot.WebSocket;
using Newtonsoft.Json;

namespace Crabot.Gateway.EventHandlers
{
    public class HelloEventHandler : IGatewayEventHandler<HeartbeatEvent>
    {
        private readonly IConnectionManager _connectionManager;

        public HelloEventHandler(IConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public async Task HandleAsync(object eventData)
        {
            var heartbeatInterval = JsonConvert.DeserializeObject<HeartbeatEvent>(eventData.ToString())
                .HeartbeatInterval;

            _connectionManager.RunHeartbeat(heartbeatInterval);

            await Task.CompletedTask;
        }
    }
}
