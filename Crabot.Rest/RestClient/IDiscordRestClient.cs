using System.Threading.Tasks;

namespace Crabot.Rest.RestClient
{
    public interface IDiscordRestClient
    {
        Task<string> GetGatewayUrlAsync();
        Task PostMessage(string channelId, string message);
    }
}