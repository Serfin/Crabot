using System.Collections.Generic;
using System.Threading.Tasks;
using Crabot.Contracts;
using Crabot.Rest.Models;

namespace Crabot.Rest.RestClient
{
    public interface IDiscordRestClient
    {
        Task<string> GetGatewayUrlAsync();
        Task<OperationResult<GatewayMessage>> AddReactionToMessage(string channelId,
            string messageId, Emoji emoji);
        Task<List<OperationResult<GatewayMessage>>> AddReactionToMessage(string channelId,
            string messageId, IEnumerable<Emoji> emojis);
        Task<OperationResult<GatewayMessage>> PostMessage(string channelId, Message message);
        Task<OperationResult<GatewayMessage>> PostMessage(string channelId, string contentMessage);
        Task<OperationResult<GatewayMessage>> EditMessage(string channelId, string messageId, 
            Message message);
    }
}