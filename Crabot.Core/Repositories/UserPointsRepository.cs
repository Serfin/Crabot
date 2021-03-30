using System;
using System.Threading.Tasks;

namespace Crabot.Core.Repositories
{
    public class UserPointsRepository : IUserPointsRepository
    {
        public Task AddBalanceToUserAccount(string userId, float amountToAdd)
        {
            throw new NotImplementedException();
        }

        public Task<float> GetUserBalanceAsync(string userId)
        {
            return Task.FromResult(0.0f);
        }

        public Task SubtractBalanceFromUserAccount(string userId, float amountToSubtract)
        {
            throw new NotImplementedException();
        }
    }
}
