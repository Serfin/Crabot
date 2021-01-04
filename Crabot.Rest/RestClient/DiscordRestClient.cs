using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
        
        public async Task<OperationResult<Contracts.Message>> PostMessage(string channelId, Message message)
        {
            try
            {
                _logger.LogWarning(JsonConvert.SerializeObject(message));

                var response = await _httpClient.PostAsync($"channels/{channelId}/messages", 
                    new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogCritical("[RES] {0} {1} {2}", (int)response.StatusCode, response.ReasonPhrase, 
                        await response.Content.ReadAsStringAsync());

                    return new OperationResult<Contracts.Message>(null, response.StatusCode,
                        response.ReasonPhrase, await response.Content.ReadAsStringAsync());
                }
                else
                {
                    return new OperationResult<Contracts.Message>(JsonConvert.DeserializeObject<Contracts.Message>(
                        await response.Content.ReadAsStringAsync()), response.StatusCode, 
                        response.ReasonPhrase, string.Empty);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical("[ERR] Error during sending request ", ex.Message);
                throw;
            }
        }

        public async Task<OperationResult<Contracts.Message>> EditMessage(string channelId, string messageId, Message message)
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

                    return new OperationResult<Contracts.Message>(null, response.StatusCode,
                        response.ReasonPhrase, await response.Content.ReadAsStringAsync());
                }
                else
                {
                    return new OperationResult<Contracts.Message>(JsonConvert.DeserializeObject<Contracts.Message>(
                        await response.Content.ReadAsStringAsync()), response.StatusCode,
                        response.ReasonPhrase, string.Empty);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical("[ERR] Error during sending request ", ex.Message);
                throw;
            }
        }
    }
}