using System.Threading.Tasks;

namespace Crabot.Core.Repositories
{
    public interface ITrackedMessageRepository
    {
        Task AddTrackedMessageAsync(TrackedMessage trackedMessage);
        Task<TrackedMessage> GetTrackedMessageAsync(string messageId);
        Task RemoveTrackedMessageAsync(string channelId, string messageId);
    }
}
