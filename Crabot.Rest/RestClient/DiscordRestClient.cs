using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Crabot.Contracts;
using Crabot.Rest.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Crabot.Rest.RestClient
{
    public class DiscordRestClient : IDiscordRestClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public DiscordRestClient(HttpClient httpClient, 
            ILogger<DiscordRestClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger; 
        }

        public async Task<string> GetGatewayUrlAsync()
        {
            try 
            {
                _logger.LogInformation("Fetching gateway url...");
                var gatewayUrlRequest = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "gateway"));

                if (gatewayUrlRequest.IsSuccessStatusCode)
                {
                    var gatewayEvent = JsonConvert.DeserializeObject<GatewayUrl>(await gatewayUrlRequest.Content.ReadAsStringAsync());
                    var gatewayUrl = string.Concat(gatewayEvent.Url, $"/?v=8&encoding=json");

                    _logger.LogInformation("Using Gateway URL - {0}", gatewayUrl);

                    return gatewayUrl;
                }
                else 
                {
                    throw new ApplicationException($"Cannot fetch Gateway address from - {_httpClient.BaseAddress}/gateway");
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Cannot fetch Gateway address ", ex.Message);
                throw;
            }
        }

        public async Task<OperationResult<GatewayMessage>> PostMessage(string channelId, string contentMessage)
        {
            return await PostMessage(channelId, new Message { Content = contentMessage });
        }
        
        public async Task<OperationResult<GatewayMessage>> PostMessage(string channelId, Message message)
        {
            try
            {
                _logger.LogWarning(JsonConvert.SerializeObject(message));

                var response = await _httpClient.PostAsync($"channels/{channelId}/messages", 
                    new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("[RES] {0} {1} {2}", (int)response.StatusCode, response.ReasonPhrase, 
                        await response.Content.ReadAsStringAsync());

                    if (response.StatusCode == HttpStatusCode.TooManyRequests)
                    {
                        var rateLimit = JsonConvert.DeserializeObject<RateLimit>(
                            await response.Content.ReadAsStringAsync());

                        return await RetryPostMessage(rateLimit, channelId, message);
                    }
                    
                    return new OperationResult<GatewayMessage>(null, response.StatusCode,
                        response.ReasonPhrase, await response.Content.ReadAsStringAsync());
                }
                else
                {
                    return new OperationResult<GatewayMessage>(JsonConvert.DeserializeObject<GatewayMessage>(
                        await response.Content.ReadAsStringAsync()), response.StatusCode, 
                        response.ReasonPhrase, string.Empty);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical("[ERR] Error during sending POST request ", ex.Message);
                throw;
            }
        }

        public async Task<OperationResult<GatewayMessage>> EditMessage(string channelId, 
            string messageId, Message message)
        {
            try
            {
                _logger.LogWarning(JsonConvert.SerializeObject(message));

                var response = await _httpClient.PatchAsync($"channels/{channelId}/messages/{messageId}",
                    new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogCritical("[RES] {0} {1} {2}", (int)response.StatusCode, response.ReasonPhrase,
                        await response.Content.ReadAsStringAsync());

                    if (response.StatusCode == HttpStatusCode.TooManyRequests)
                    {
                        var rateLimit = JsonConvert.DeserializeObject<RateLimit>(
                            await response.Content.ReadAsStringAsync());

                        return await RetryEditMessage(rateLimit, channelId, messageId, message);
                    }

                    return new OperationResult<GatewayMessage>(null, response.StatusCode,
                        response.ReasonPhrase, await response.Content.ReadAsStringAsync());
                }
                else
                {
                    return new OperationResult<GatewayMessage>(JsonConvert.DeserializeObject<GatewayMessage>(
                        await response.Content.ReadAsStringAsync()), response.StatusCode,
                        response.ReasonPhrase, string.Empty);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical("[ERR] Error during sending PATCH request ", ex.Message);
                throw;
            }
        }

        public async Task<List<OperationResult<GatewayMessage>>> AddReactionToMessage(string channelId,
            string messageId, IEnumerable<Emoji> emojis)
        {
            var result = new List<OperationResult<GatewayMessage>>();
            foreach (var emocjiReactionRequest in emojis)
            {
                result.Add(await AddReactionToMessage(channelId, messageId, emocjiReactionRequest));

                await Task.Delay(250);
            }

            return result;
        }

        public async Task<OperationResult<GatewayMessage>> AddReactionToMessage(string channelId, 
            string messageId, Emoji emoji)
        {
            try
            {
                var response = await _httpClient.PutAsync($"channels/{channelId}/messages/{messageId}/reactions/%3A{emoji.Name}%3A{emoji.Id}/@me", null);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogCritical("[RES] {0} {1} {2}", (int)response.StatusCode, response.ReasonPhrase,
                        await response.Content.ReadAsStringAsync());

                    if (response.StatusCode == HttpStatusCode.TooManyRequests)
                    {
                        var rateLimit = JsonConvert.DeserializeObject<RateLimit>(
                            await response.Content.ReadAsStringAsync());

                        return await RetryPutReaction(rateLimit, channelId, messageId, emoji);
                    }

                    return new OperationResult<GatewayMessage>(null, response.StatusCode,
                        response.ReasonPhrase, await response.Content.ReadAsStringAsync());
                }
                else
                {
                    return new OperationResult<GatewayMessage>(JsonConvert.DeserializeObject<GatewayMessage>(
                        await response.Content.ReadAsStringAsync()), response.StatusCode,
                        response.ReasonPhrase, string.Empty);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical("[ERR] Error during sending PUT request ", ex.Message);
                throw;
            }
        }
    
        private async Task<OperationResult<GatewayMessage>> RetryPostMessage(RateLimit rateLimit,
            string channelId, Message message)
        {
            var retryAfter = (int)rateLimit.RetryAfter * 1000;
            await Task.Delay(retryAfter);

            _logger.LogWarning("Retrying POST request after rate limit - {0}ms", retryAfter);

            return await PostMessage(channelId, message);
        }

        private async Task<OperationResult<GatewayMessage>> RetryEditMessage(RateLimit rateLimit,
            string channelId, string messageId, Message message)
        {
            var retryAfter = (int)rateLimit.RetryAfter * 1000;
            await Task.Delay(retryAfter);

            _logger.LogWarning("Retrying PATCH request after rate limit - {0}ms", retryAfter);

            return await EditMessage(channelId, messageId, message);
        }

        private async Task<OperationResult<GatewayMessage>> RetryPutReaction(RateLimit rateLimit,
            string channelId, string messageId, Emoji emoji)
        {
            var retryAfter = (int)rateLimit.RetryAfter * 1000;
            await Task.Delay(retryAfter);

            _logger.LogWarning("Retrying PATCH request after rate limit - {0}ms", retryAfter);

            return await AddReactionToMessage(channelId, messageId, emoji);
        }
    }
}