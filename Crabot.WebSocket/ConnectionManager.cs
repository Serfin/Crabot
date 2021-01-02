using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Crabot.Core.Events;
using Crabot.Core.Repositories;
using Crabot.Rest.RestClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Crabot.WebSocket
{
    public class ConnectionManager : IConnectionManager
    {
        public int? SequenceNumber { get; private set; }

        private readonly ILogger _logger;
        private readonly IDiscordSocketClient _discordSocketClient;
        private readonly IDiscordRestClient _discordRestClient;
        private readonly IClientInfoRepository _clientInfoRepository;

        private CancellationTokenSource _heartbeatTokenSource;
        private CancellationToken _heartbeatToken;
        public Task _heartbeatTask;

        public event Func<GatewayPayload, Task> EventReceive;

        public ConnectionManager(
            ILogger<ConnectionManager> logger,
            IDiscordSocketClient discordSocketClient,
            IDiscordRestClient discordRestClient,
            IClientInfoRepository clientInfoRepository)
        {
            _logger = logger;
            _discordSocketClient = discordSocketClient;
            _discordSocketClient.MessageReceive += OnMessageReceive;
            _discordRestClient = discordRestClient;
            _clientInfoRepository = clientInfoRepository;
            _heartbeatToken = CancellationToken.None;
        }

        private async Task OnMessageReceive(string message)
        {
            var payload = JsonConvert.DeserializeObject<GatewayPayload>(message);
            SetSequenceNumber(payload.SequenceNumber);

            if (payload.Opcode == GatewayOpCode.Hello)
            {
                _logger.LogInformation("[{0}]", payload.Opcode);
                await _discordRestClient.PostMessage("764840399696822322", "```\nServer sent [Hello]\n```");
                SetCancellationToken();

                var heartbeatInterval = JsonConvert.DeserializeObject<HeartbeatEvent>(
                   payload.EventData.ToString()).HeartbeatInterval;

                _heartbeatTask = RunHeartbeat(heartbeatInterval);
            }
            else if (payload.Opcode == GatewayOpCode.Reconnect)
            {
                _logger.LogInformation("[{0}]", payload.Opcode);
                await _discordRestClient.PostMessage("764840399696822322", "```\nServer requested [Reconnect]\n```");

                CloseHeartbeating();

                var gatewayUrl = await _discordRestClient.GetGatewayUrlAsync();
                await CreateConnectionAsync(new Uri(gatewayUrl));
            }
            else if (payload.Opcode == GatewayOpCode.HeartbeatAck)
            {
                _logger.LogWarning("Session prolongate successful!");
            }
            else if (payload.Opcode == GatewayOpCode.Dispatch && payload.EventName == "RESUMED")
            {
                await _discordRestClient.PostMessage("764840399696822322", "```\nServer sent [Dispatch - RESUMED]\n```");
                _logger.LogWarning("Session resumed!");
            }
            else if (payload.Opcode == GatewayOpCode.InvalidSession)
            {
                _logger.LogWarning("Cannot resume session!");
                await _discordRestClient.PostMessage("764840399696822322", "```\nServer sent [InvalidSession]\n```");

                _logger.LogInformation("[{0}]", payload.Opcode);
                CloseHeartbeating();

                var conversionSuccess = bool.TryParse(payload.EventData.ToString(), out bool canBeResumed);

                if (conversionSuccess && canBeResumed)
                {
                    await _discordRestClient.PostMessage("764840399696822322", "```\nSession can be resumed\n```");

                    var gatewayUrl = await _discordRestClient.GetGatewayUrlAsync();
                    await CreateConnectionAsync(new Uri(gatewayUrl));
                }
                else
                {
                    // Delete old session id
                    _clientInfoRepository.DeleteClientInfo();
                    await _discordRestClient.PostMessage("764840399696822322", "```\nSession cannot be resumed\n```");
                    await Task.Delay(new Random().Next(1, 6) * 1000);

                    var gatewayUrl = await _discordRestClient.GetGatewayUrlAsync();
                    await CreateConnectionAsync(new Uri(gatewayUrl));
                }
            }
            else
            {
                await EventReceive.Invoke(payload);
            }
        }

        public void SetSequenceNumber(int? sequenceNumber)
        {
            if (sequenceNumber.HasValue)
            {
                SequenceNumber = sequenceNumber.Value;
            }
        }

        private void SetCancellationToken()
        {
            _heartbeatTokenSource = new CancellationTokenSource();
            _heartbeatToken = _heartbeatTokenSource.Token;
        }

        private void CloseHeartbeating()
        {
            _logger.LogWarning("Requested heartbeat close!");
            _heartbeatTokenSource.Cancel(false);
        }

        public async Task RunHeartbeat(int heartbeatInterval)
        {
            _logger.LogInformation("Starting heartbeat Task! - interval {0}ms", heartbeatInterval);
            await _discordRestClient.PostMessage("764840399696822322", 
                string.Format("```\nStarting heartbeat Task! - interval {0}ms\n```", heartbeatInterval));

            while (!_heartbeatToken.IsCancellationRequested)
            {
                var heartbeatEvent = new GatewayPayload
                {
                    Opcode = GatewayOpCode.Heartbeat,
                    EventData = SequenceNumber ?? null,
                };

                var heartbeatEventBytes = Encoding.UTF8.GetBytes(
                    JsonConvert.SerializeObject(heartbeatEvent));
                await _discordSocketClient.SendAsync(heartbeatEventBytes, true);

                await Task.Delay(heartbeatInterval, _heartbeatToken);
            }

            _logger.LogWarning("Client stopped heartbeating");
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

        public async Task CreateConnectionAsync(Uri gatewayUri)
        {
            var clientInfo = _clientInfoRepository.GetClientInfo();
            if (clientInfo?.SessionId != null)
            {
                // Resume session
                await _discordSocketClient.DisconnectAsync();
                await _discordSocketClient.ConnectAsync(gatewayUri);
                await ResumeSession(clientInfo.SessionId);
            }
            else
            {
                // Create new session
                // Disconnect client for safety (could be in connected state after InvalidSession event)
                await _discordSocketClient.DisconnectAsync();
                await _discordSocketClient.ConnectAsync(gatewayUri);
                await IdentifyClient();
            }
        }
    }
}
