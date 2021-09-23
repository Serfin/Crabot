using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Crabot.Core.Events;
using Crabot.Core.Events.Voice;
using Crabot.Core.Repositories;
using Crabot.Rest.RestClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Crabot.WebSocket
{
    public class VoiceGatewayConnection : IGatewayConnection
    {
        private readonly ILogger _logger;
        private readonly IDiscordSocketClient _discordSocketClient;
        private readonly IDiscordRestClient _discordRestClient;
        private readonly IClientInfoRepository _clientInfoRepository;
        private readonly ConcurrentQueue<long> _heartbeatTimes;

        private CancellationTokenSource _heartbeatTokenSource;
        private CancellationToken _heartbeatToken;
        private Task _heartbeatTask;

        private VoiceServerUpdate _voiceServerUpdate;
        private VoiceStateUpdate _voiceStateUpdate;

        public event Func<GatewayPayload, Task> EventReceive;

        private int? _sequenceNumber = default(int);

        public VoiceGatewayConnection(
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
            await Task.CompletedTask;

            _logger.LogError(message);
        }

        public async Task StartConnectionAsync()
        {
            if (_voiceServerUpdate is null || _voiceStateUpdate is null) return;

            var clientInfo = _clientInfoRepository.GetClientInfo();
            var gatewayUrl = await _discordRestClient.GetGatewayUrlAsync();

            if (clientInfo?.SessionId != null)
            {
                // Resume session
                await _discordSocketClient.DisconnectAsync(WebSocketCloseStatus.NormalClosure);
                await _discordSocketClient.ConnectAsync(new Uri(gatewayUrl));
                //await ResumeSession(clientInfo.SessionId);
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

        private async Task IdentifyClient()
        {
            var identifyEvent = new GatewayPayload
            {
                Opcode = (int)GatewayVoiceOpCode.Identify,
                EventData = new VoiceIdentify
                {
                    UserId = _voiceStateUpdate.UserId,
                    ServerId = _voiceStateUpdate.GuildId,
                    SessionId = _voiceStateUpdate.SessionId,
                    Token = _voiceServerUpdate.Token
                }
            };

            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(identifyEvent));

            await _discordSocketClient.SendAsync(bytes, true);
        }

        public void SetVoiceStateUpdate(VoiceStateUpdate voiceStateUpdate)
        {
            _voiceStateUpdate = voiceStateUpdate;
        }

        public void SetVoiceServerUpdate(VoiceServerUpdate voiceServerUpdate)
        {
            _voiceServerUpdate = voiceServerUpdate;
        }

        public Task RunHeartbeat(int heartbeatInterval)
        {
            throw new NotImplementedException();
        }

        public void SetSequenceNumber(int? sequenceNumber)
        {
            if (sequenceNumber.HasValue)
            {
                _sequenceNumber = sequenceNumber.Value;
            }
        }
    }
}
