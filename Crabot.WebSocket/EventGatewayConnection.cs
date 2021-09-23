using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
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
    public class EventGatewayConnection : IGatewayConnection
    {
        private readonly ILogger _logger;
        private readonly IDiscordSocketClient _discordSocketClient;
        private readonly IDiscordRestClient _discordRestClient;
        private readonly IClientInfoRepository _clientInfoRepository;
        private readonly ConcurrentQueue<long> _heartbeatTimes;

        private CancellationTokenSource _heartbeatTokenSource;
        private CancellationToken _heartbeatToken;
        private Task _heartbeatTask;

        public event Func<GatewayPayload, Task> EventReceive;

        private int? _sequenceNumber = default(int);
        private long _lastMessageTime;
        private int _latency;

        public EventGatewayConnection(
            ILogger<EventGatewayConnection> logger,
            IDiscordRestClient discordRestClient,
            IClientInfoRepository clientInfoRepository)
        {
            _logger = logger;
            _discordSocketClient = new DiscordSocketClient();
            _discordSocketClient.MessageReceive += OnMessageReceive;
            _discordRestClient = discordRestClient;
            _clientInfoRepository = clientInfoRepository;
            _heartbeatToken = CancellationToken.None;
            _heartbeatTimes = new ConcurrentQueue<long>();
        }

        private async Task OnMessageReceive(string message)
        {
            var payload = JsonConvert.DeserializeObject<GatewayPayload>(message);

            if (payload.EventName is null)
            {
                _logger.LogInformation("[{0}]", (GatewayOpCode)payload.Opcode);
            }
            else
            {
                _logger.LogInformation("[{0} - {1}]", (GatewayOpCode)payload.Opcode, payload.EventName);
            }

            SetSequenceNumber(payload.SequenceNumber);
            _lastMessageTime = Environment.TickCount;

            switch ((GatewayOpCode)payload.Opcode)
            {
                case GatewayOpCode.Hello:
                    {
                        SetCancellationToken();

                        var heartbeatInterval = JsonConvert.DeserializeObject<HeartbeatEvent>(
                           payload.EventData.ToString()).HeartbeatInterval;

                        _heartbeatTask = RunHeartbeat(heartbeatInterval);
                    }
                    break;
                case GatewayOpCode.Reconnect:
                    {
                        CloseHeartbeating();
                        await StartConnectionAsync();
                    }
                    break;
                case GatewayOpCode.Heartbeat:
                    {
                        _logger.LogWarning("Server requested manual heartbeat!");

                        var heartbeatEvent = new GatewayPayload
                        {
                            Opcode = (int)GatewayOpCode.Heartbeat,
                            EventData = _sequenceNumber ?? null,
                        };

                        var heartbeatEventBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(heartbeatEvent));
                        await _discordSocketClient.SendAsync(heartbeatEventBytes, true);
                    }
                    break;
                case GatewayOpCode.HeartbeatAck:
                    {
                        if (_heartbeatTimes.TryDequeue(out long time))
                        {
                            int latency = (int)(Environment.TickCount - time);
                            int before = _latency;
                            _latency = latency;

                            _logger.LogWarning("Latency: old - {0}ms | new - {1}ms", before, _latency);
                        }
                    }
                    break;
                case GatewayOpCode.InvalidSession:
                    {
                        _logger.LogWarning("Cannot resume session!");

                        CloseHeartbeating();
                        if (bool.TryParse(payload.EventData.ToString(), out bool canBeResumed) && canBeResumed)
                        {
                            await StartConnectionAsync();
                        }
                        else
                        {
                            // Delete old session id
                            _clientInfoRepository.DeleteClientInfo();
                            SetSequenceNumber(0);
                            await Task.Delay(new Random().Next(1, 6) * 1000);
                            await StartConnectionAsync();
                        }
                    }
                    break;
                case GatewayOpCode.Dispatch:
                    {
                        await EventReceive.Invoke(payload);
                    }
                    break;
                default:
                    _logger.LogWarning("Received unhandled event! - {0} {1} {2}", payload.Opcode.ToString().ToUpperInvariant(),
                        payload.EventName ?? string.Empty, payload.EventData ?? string.Empty);
                    break;
            }
        }

        private void SetSequenceNumber(int? sequenceNumber)
        {
            if (sequenceNumber.HasValue)
            {
                _sequenceNumber = sequenceNumber.Value;
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

        private async Task RunHeartbeat(int heartbeatInterval)
        {
            _logger.LogInformation("Starting heartbeating - interval {0}ms", heartbeatInterval);

            while (!_heartbeatToken.IsCancellationRequested)
            {
                int now = Environment.TickCount;
                if (_heartbeatTimes.Count != 0 && (now - _lastMessageTime) > heartbeatInterval)
                {
                    _logger.LogCritical("Did not receive HeartbeatAck");
                    CloseHeartbeating();
                    await StartConnectionAsync();

                    break;
                }

                _heartbeatTimes.Enqueue(now);

                var heartbeatEvent = new GatewayPayload
                {
                    Opcode = (int)GatewayOpCode.Heartbeat,
                    EventData = _sequenceNumber ?? null,
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
                Opcode = (int)GatewayOpCode.Identify,
                EventData = new IdentifyEvent
                {
                    Token = Environment.GetEnvironmentVariable("BOT_TOKEN"),
                    Intents = 1791,
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
                Opcode = (int)GatewayOpCode.Resume,
                EventData = new ResumeEvent
                {
                    Token = Environment.GetEnvironmentVariable("BOT_TOKEN"),
                    SessionId = sessionId,
                    Sequence = _sequenceNumber.Value
                }
            };

            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(resumeEvent));
            await _discordSocketClient.SendAsync(bytes, true);
        }

        public async Task StartConnectionAsync()
        {
            var clientInfo = _clientInfoRepository.GetClientInfo();
            var gatewayUrl = await _discordRestClient.GetGatewayUrlAsync();

            if (clientInfo?.SessionId != null)
            {
                // Resume session
                await _discordSocketClient.DisconnectAsync(WebSocketCloseStatus.NormalClosure);
                await _discordSocketClient.ConnectAsync(new Uri(gatewayUrl));
                await ResumeSession(clientInfo.SessionId);
            }
            else
            {
                // Create new session
                // Disconnect client for safety (could be in connected state after InvalidSession event)
                await _discordSocketClient.DisconnectAsync(WebSocketCloseStatus.NormalClosure);
                await _discordSocketClient.ConnectAsync(new Uri(gatewayUrl));
                await IdentifyClient();
            }
        }
    }
}
