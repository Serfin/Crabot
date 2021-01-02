using System.Threading.Tasks;
using Crabot.Rest.Models;

namespace Crabot.Rest.RestClient
{
    public interface IDiscordRestClient
    {
        Task<string> GetGatewayUrlAsync();
        Task PostMessage(string channelId, Message message);
    }
}