using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Crabot.Core.Repositories;
using Crabot.Gateway;
using Crabot.Gateway.SocketClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Crabot.Core
{
    public class ConnectionManager : IConnectionManager
    {
        public int? SequenceNumber { get; private set; }

        private readonly ILogger _logger;
        private readonly IDiscordSocketClient _discordSocketClient;
        private readonly IClientInfoRepository _clientInfoRepository;

        private CancellationTokenSource _heartbeatTokenSource;
        private CancellationToken _heartbeatToken;

        public ConnectionManager(
            ILogger<ConnectionManager> logger,
            IDiscordSocketClient discordSocketClient, 
            IClientInfoRepository clientInfoRepository)
        {
            _logger = logger;
            _discordSocketClient = discordSocketClient;
            _clientInfoRepository = clientInfoRepository;

            _heartbeatToken = CancellationToken.None;
        }

        public void SetSequenceNumber(int? sequenceNumber)
        {
            if (sequenceNumber.HasValue)
            {
                SequenceNumber = sequenceNumber.Value;
            }
        }

        private void RequestHeartbeatCancellation()
        {
            try
            {
                _heartbeatTokenSource.Cancel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot close heartbeat Task");
            }
        }

        private void RunHeartbeat(int heartbeatInterval)
        {
            _heartbeatTokenSource = new CancellationTokenSource();
            _heartbeatToken = _heartbeatTokenSource.Token;

            _ = Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(heartbeatInterval);

                    var heartbeatEvent = new GatewayPayload
                    {
                        Opcode = GatewayOpCode.Heartbeat,
                        EventData = SequenceNumber ?? null,
                    };

                    var heartbeatEventBytes = Encoding.UTF8.GetBytes(
                        JsonConvert.SerializeObject(heartbeatEvent));
                    await _discordSocketClient.SendAsync(heartbeatEventBytes, true);
                }
            }, _heartbeatToken);
        }

        private async Task IdentifyClient()
        {
            var identityEvent = new GatewayPayload
            {
                Opcode = GatewayOpCode.Identify,
                EventData = new IdentifyEvent
                {
                    Token = Environment.GetEnvironmentVariable("BOT_TOKEN"),
                    Intents = 513,
                    Compress = false,
                    Properties = new Dictionary<string, string> {
                            { "$os", "windows" },
                            { "$browser", "crabot" },
                            { "$device", "crabot"}
                    }
                }
            };

            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(identityEvent));
            await _discordSocketClient.SendAsync(bytes, true);
        }

        private async Task ResumeSession(string sessionId)
        {
            var resumeEvent = new GatewayPayload
            {
                Opcode = GatewayOpCode.Resume,
                EventData = new ResumeEvent
                {
                    Token = Environment.GetEnvironmentVariable("BOT_TOKEN"),
                    SessionId = sessionId,
                    Sequence = SequenceNumber.Value
                }
            };

            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(resumeEvent));
            await _discordSocketClient.SendAsync(bytes, true);
        }

        public async Task CreateConnection(GatewayPayload helloEvent)
        {
            var clientInfo = _clientInfoRepository.GetClientInfo();

            if (clientInfo?.SessionId != null)
            {
                // Resume session
                RequestHeartbeatCancellation();
                await _discordSocketClient.ReconnectAsync();

                var heartbeat = JsonConvert.DeserializeObject<HeartbeatEvent>(
                    helloEvent.EventData.ToString());

                RunHeartbeat(heartbeat.HeartbeatInterval);
                await IdentifyClient();
                await ResumeSession(clientInfo.SessionId);
            }
            else
            {
                // Create new session
                var heartbeat = JsonConvert.DeserializeObject<HeartbeatEvent>(
                    helloEvent.EventData.ToString());

                RunHeartbeat(heartbeat.HeartbeatInterval);
                await IdentifyClient();
            }
        }
    }
}
