using System.Threading.Tasks;

namespace Crabot.Core.Repositories
{
    public interface ITrackedMessageRepository
    {
        Task AddTrackedMessageAsync(TrackedMessage trackedMessage);
        Task RemoveTrackedMessageAsync(TrackedMessage trackedMessage);
    }
}
