using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Crabot.Gateway.SocketClient
{
    public class DiscordSocketClient : IDiscordSocketClient
    {
        public int? SequenceNumber { get; set; }

        public const int ReceiveChunkSize = 16 * 1024; //16KB
        private CancellationTokenSource _disconnectTokenSource, _cancelTokenSource;
        private CancellationToken _cancelToken, _parentToken;
        private ClientWebSocket _client;
        private Task _task;
        public event Func<GatewayPayload, Task> TextMessage;
        private readonly ILogger _logger;

        public DiscordSocketClient(ILogger<DiscordSocketClient> logger)
        {
            _logger = logger;
            _cancelToken = CancellationToken.None;
            _parentToken = CancellationToken.None;
        }

        public async Task ConnectAsync(string address)
        {
            _client = new ClientWebSocket();
            _disconnectTokenSource = new CancellationTokenSource();
            _cancelTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_parentToken, 
                _disconnectTokenSource.Token);
            _cancelToken = _cancelTokenSource.Token;

            _logger.LogInformation("Connecting to Gateway...");

            // Connect to the socket and start listening
            await _client.ConnectAsync(new Uri(address), _cancelToken);
            _task = RunAsync(_cancelToken);
        }

        public void RequestCancellation()
        {
            if (_cancelToken.CanBeCanceled)
            {
                _cancelTokenSource.Cancel();
            }
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Listening on Gateway socket...");
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var dataBufferBytes = new ArraySegment<byte>(new byte[ReceiveChunkSize]);
                
                    WebSocketReceiveResult socketResult = 
                        await _client.ReceiveAsync(dataBufferBytes, CancellationToken.None);

                    if (socketResult.MessageType == WebSocketMessageType.Close)
                    {
                        throw new WebSocketException((int)socketResult.CloseStatus, 
                            socketResult.CloseStatusDescription);
                    }

                    string text = Encoding.UTF8.GetString(dataBufferBytes.Array, 0, socketResult.Count);
                    var gatewayPayload = JsonConvert.DeserializeObject<GatewayPayload>(text);

                    SequenceNumber = gatewayPayload.SequenceNumber;

                    await TextMessage(gatewayPayload);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(), ex, "");
                throw;
            }
        }

        public async Task SendAsync(byte[] payload, bool isEoF)
        {
            if (_client.State == WebSocketState.Closed)
            {
                throw new WebSocketException((int)_client.CloseStatus, _client.CloseStatusDescription);
            }

            await _client.SendAsync(new ArraySegment<byte>(payload), WebSocketMessageType.Text,
                isEoF, _cancelToken);

            _logger.LogInformation("Sent data to gateway - {0}", Encoding.UTF8.GetString(payload));
        }
    }
}