using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crabot.Core.Repositories
{
    public interface IUserPointsRepository
    {
        Task<float?> GetUserBalanceAsync(string userId);
        Task<List<UserPoint>> GetUsersAsync();
        Task AddBalanceToUserAccount(string userId, float amountToAdd);
        Task AddUserToSystem(string nickname, string userId);
        Task SubtractBalanceFromUserAccount(string userId, float amountToSubtract);
        Task<bool> CanUseDailyPoints(string userId);
        Task UpdateDailyPoints(string userId);
    }
}
