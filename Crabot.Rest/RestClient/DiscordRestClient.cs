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

        public async Task PostMessage(string channelId, string message)
        {
            try
            {
                var response = await _httpClient.PostAsync($"channels/{channelId}/messages", 
                    new StringContent(JsonConvert.SerializeObject(new Message { Content = message }), 
                    Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogCritical("[RES] {0} {1} {2}", (int)response.StatusCode, response.ReasonPhrase, 
                        await response.Content.ReadAsStringAsync());
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