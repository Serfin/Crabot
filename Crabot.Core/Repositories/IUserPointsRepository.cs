using System.Threading.Tasks;

namespace Crabot.Core.Repositories
{
    public interface IUserPointsRepository
    {
        Task<float> GetUserBalanceAsync(string userId);
        Task AddBalanceToUserAccount(string userId, float amountToAdd);
        Task SubtractBalanceFromUserAccount(string userId, float amountToSubtract);
    }
}
