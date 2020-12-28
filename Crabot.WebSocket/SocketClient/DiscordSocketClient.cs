using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Crabot.WebSocket
{
    public class DiscordSocketClient : IDiscordSocketClient
    {
        public const int ReceiveChunkSize = 16 * 1024; //16KB
        private CancellationTokenSource _disconnectTokenSource, _cancelTokenSource;
        private CancellationToken _cancelToken, _parentToken;
        private ClientWebSocket _client;
        private Task _task;
        public event Func<string, Task> MessageReceive;
        private readonly ILogger _logger;

        public DiscordSocketClient(ILogger<DiscordSocketClient> logger)
        {
            _logger = logger;

            _cancelToken = CancellationToken.None;
            _parentToken = CancellationToken.None;
        }

        public async Task ConnectAsync(Uri address)
        {
            try
            {
                _client = new ClientWebSocket();
                _disconnectTokenSource = new CancellationTokenSource();
                _cancelTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_parentToken, 
                    _disconnectTokenSource.Token);
                _cancelToken = _cancelTokenSource.Token;

                _logger.LogInformation("Connecting to Gateway...");

                // Connect to the socket and start listening
                await _client.ConnectAsync(address, _cancelToken);
                _task = StartListeningAsync(_cancelToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during connection opening!");
            }
        }

        public async Task CloseAsync()
        {
            try
            {
                _logger.LogWarning("Closing existing connection!");
                await _client.CloseAsync(WebSocketCloseStatus.EndpointUnavailable, 
                    "Client requested reconnect!", _cancelToken);
                RequestListeningCancellation();

                _client.Dispose();
                _client = null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during closing conncetion!");
            }
        }

        private void RequestListeningCancellation()
        {
            try
            {
                _cancelTokenSource.Cancel(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot close listening Task");
            }
        }

        public async Task StartListeningAsync(CancellationToken cancellationToken)
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
                    
                    await MessageReceive(text);
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