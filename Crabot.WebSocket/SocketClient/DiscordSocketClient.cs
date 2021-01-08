using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Crabot.WebSocket
{
    public class DiscordSocketClient : IDiscordSocketClient
    {
        public event Func<string, Task> MessageReceive;

        private const int ReceiveChunkSize = 16 * 1024; //16KB
        private readonly ILogger _logger;
        private readonly SemaphoreSlim _socketLock = new SemaphoreSlim(1, 1);

        private CancellationTokenSource _disconnectTokenSource, _cancelTokenSource;
        private CancellationToken _cancelToken, _parentToken;
        private ClientWebSocket _client;
        private Task _task;

        public DiscordSocketClient(ILogger<DiscordSocketClient> logger)
        {
            _logger = logger;

            _cancelToken = CancellationToken.None;
            _parentToken = CancellationToken.None;
        }

        /// <summary>
        /// Connect to specified Uri and start listening for events
        /// </summary>
        /// <param name="address">socket Uri</param>
        /// <returns>Connect Task</returns>
        public async Task ConnectAsync(Uri address)
        {
            await _socketLock.WaitAsync();
            _logger.LogInformation($"Connecting to {address}...");

            try
            {
                _disconnectTokenSource?.Dispose();
                _cancelTokenSource?.Dispose();

                _disconnectTokenSource = new CancellationTokenSource();
                _cancelTokenSource = CancellationTokenSource
                    .CreateLinkedTokenSource(_parentToken, _disconnectTokenSource.Token);
                _cancelToken = _cancelTokenSource.Token;

                _client?.Dispose();
                _client = new ClientWebSocket();

                await _client.ConnectAsync(address, _cancelToken);
                _task = RunAsync(_cancelToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during connection opening!");
            }
            finally
            {
                _socketLock.Release();
            }
        }

        /// <summary>
        /// Disconnect from socket and stop listening task
        /// </summary>
        /// <returns>Disconnect Task</returns>
        public async Task DisconnectAsync()
        {
            await _socketLock.WaitAsync();

            try
            {
                if (_client != null)
                {
                    _logger.LogWarning("Closing existing connection!");
                    _cancelTokenSource.Cancel(false);
                    _disconnectTokenSource.Cancel(false);

                    await _client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);

                    _client.Dispose();
                    _client = null;
                }

                //await (_task ?? Task.Delay(0));
                _task = null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during closing conncetion!");
            }
            finally
            {
                _socketLock.Release();
            }
        }

        /// <summary>
        /// Start listening on socket for data
        /// </summary>
        /// <param name="cancelToken">Cancellation token</param>
        /// <returns>Listen on socket Task</returns>
        public async Task RunAsync(CancellationToken cancelToken)
        {
            _logger.LogInformation("Listening on socket...");

            // Initial 16KB buffer
            var messageBuffer = new ArraySegment<byte>(new byte[ReceiveChunkSize]);

            try
            {
                while (!cancelToken.IsCancellationRequested)
                {
                    WebSocketReceiveResult socketResult = await _client.ReceiveAsync(messageBuffer, CancellationToken.None);

                    byte[] longMessageBuffer;
                    int bufferLength;

                    if (socketResult.MessageType == WebSocketMessageType.Close)
                    {
                        throw new WebSocketException((int)socketResult.CloseStatus,
                            socketResult.CloseStatusDescription);
                    }

                    if (!socketResult.EndOfMessage)
                    {
                        using (var stream = new MemoryStream())
                        {
                            // Write initial 16KB of data to long message buffer
                            stream.Write(messageBuffer.Array, 0, socketResult.Count);

                            // Append more if there is not socket EOF
                            do
                            {
                                if (cancelToken.IsCancellationRequested)
                                {
                                    return;
                                }

                                socketResult = await _client.ReceiveAsync(messageBuffer, cancelToken).ConfigureAwait(false);
                                stream.Write(messageBuffer.Array, 0, socketResult.Count);
                            }
                            while (socketResult == null || !socketResult.EndOfMessage);

                            bufferLength = (int)stream.Length;
                            longMessageBuffer = stream.TryGetBuffer(out var streamBuffer) 
                                ? streamBuffer.Array 
                                : stream.ToArray();

                        }
                    }
                    else
                    {
                        bufferLength = socketResult.Count;
                        longMessageBuffer = messageBuffer.Array;
                    }

                    if (socketResult.MessageType == WebSocketMessageType.Text)
                    {
                        string text = Encoding.UTF8.GetString(longMessageBuffer, 0, bufferLength);
                        await MessageReceive(text);
                    }
                    else
                    {
                        throw new NotSupportedException("Binary messages are not supperted");
                    }
                }
            }
            catch (WebSocketException scx)
            {
                _logger.LogError(scx, "Socket is in 'Close' state");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot start listening on socket");
                throw;
            }
        }

        /// <summary>
        /// Send bytes through the socket
        /// </summary>
        /// <param name="payload">Byte payload</param>
        /// <param name="isEoF">Is Eof</param>
        /// <returns>Send Task</returns>
        public async Task SendAsync(byte[] payload, bool isEoF)
        {
            await _socketLock.WaitAsync();

            try
            {
                if (_client == null)
                {
                    return;
                }

                if (_client.State == WebSocketState.Closed)
                {
                    throw new WebSocketException((int)_client.CloseStatus, _client.CloseStatusDescription);
                }

                await _client.SendAsync(new ArraySegment<byte>(payload), WebSocketMessageType.Text,
                    isEoF, _cancelToken);

                _logger.LogInformation("Sending data - {0}", Encoding.UTF8.GetString(payload));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during sending event");
            }
            finally
            {
                _socketLock.Release();
            }
        }
    }
}