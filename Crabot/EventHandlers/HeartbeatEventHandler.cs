using System.Text;
using System.Threading.Tasks;
using Crabot.Gateway;
using Crabot.Gateway.SocketClient;
using Newtonsoft.Json;

namespace Crabot
{
    public class HeartbeatEventHandler : IGatewayEventHandler<HeartbeatEvent>
    {
        private readonly IDiscordSocketClient _discordSocketClient;

        public HeartbeatEventHandler(
            IDiscordSocketClient discordSocketClient)
        {
            _discordSocketClient = discordSocketClient;
        }

        public async Task HandleAsync(object @event)
        {
            var payload = (GatewayPayload)@event;
            var heartbeat = JsonConvert.DeserializeObject<HeartbeatEvent>(payload.EventData.ToString());

            // Heartbeat
            var _ = Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(heartbeat.HeartbeatInterval);

                    var heartbeatEvent = new GatewayPayload
                    {
                        Opcode = GatewayOpCode.Heartbeat,
                        EventData = _discordSocketClient.SequenceNumber ?? null,
                    };

                    var heartbeatEventBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(heartbeatEvent));
                    await _discordSocketClient.SendAsync(heartbeatEventBytes, true);
                }
            });

            await Task.CompletedTask;
        }
    }
}
