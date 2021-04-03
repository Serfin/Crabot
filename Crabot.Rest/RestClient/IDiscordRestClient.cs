using System.Threading.Tasks;
using Crabot.Rest.Models;

namespace Crabot.Rest.RestClient
{
    public interface IDiscordRestClient
    {
        Task<string> GetGatewayUrlAsync();
        Task<OperationResult<Contracts.GatewayMessage>> PostMessage(string channelId, Message message);
        Task<OperationResult<Contracts.GatewayMessage>> EditMessage(string channelId, string messageId, 
            Message message);
    }
}