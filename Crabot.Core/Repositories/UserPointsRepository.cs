using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crabot.Core.Repositories
{
    public class UserPointsRepository : IUserPointsRepository
    {
        private const string filePath = "./user_points_database.txt";

        public async Task AddBalanceToUserAccount(string userId, float amountToAdd)
        {
            var usersData = ToUserPairData(await File.ReadAllTextAsync(filePath));

            usersData.FirstOrDefault(x => x.UserId == userId).Balance += amountToAdd;

            await UpdateData(usersData);
        }

        public async Task AddUserToSystem(string userId)
        {
            await File.AppendAllTextAsync(filePath, $"{userId}|0,0|{DateTime.Now.Subtract(TimeSpan.FromMinutes(10))};");
        }

        public async Task<float?> GetUserBalanceAsync(string userId)
        {
            var usersData = ToUserPairData(await File.ReadAllTextAsync(filePath));

            return usersData.FirstOrDefault(x => x.UserId == userId)?.Balance;
        }

        public async Task<List<UserPoint>> GetUsersAsync()
        {
            return ToUserPairData(await File.ReadAllTextAsync(filePath));
        }

        public async Task<bool> CanUseDailyPoints(string userId)
        {
            var usersData = ToUserPairData(await File.ReadAllTextAsync(filePath));

            return usersData.FirstOrDefault(x => x.UserId == userId)?.DailyUsedAt.AddMinutes(10) < DateTime.Now;
        }

        public async Task UpdateDailyPoints(string userId)
        {
            var usersData = ToUserPairData(await File.ReadAllTextAsync(filePath));

            usersData.FirstOrDefault(x => x.UserId == userId).DailyUsedAt = DateTime.Now;

            await UpdateData(usersData);
        }

        public async Task SubtractBalanceFromUserAccount(string userId, float amountToSubtract)
        {
            var usersData = ToUserPairData(await File.ReadAllTextAsync(filePath));

            usersData.FirstOrDefault(x => x.UserId == userId).Balance -= amountToSubtract;

            await UpdateData(usersData);
        }

        private List<UserPoint> ToUserPairData(string data)
        {
            var result = new List<UserPoint>();
            var splitBySemicolon = data.Split(';', StringSplitOptions.RemoveEmptyEntries);

            foreach (var pair in splitBySemicolon)
            {
                var userDataPair = pair.Split('|', StringSplitOptions.RemoveEmptyEntries);

                if (userDataPair.Length == 0)
                {
                    continue;
                }

                result.Add(new UserPoint(userDataPair[0], float.Parse(userDataPair[1]), DateTime.Parse(userDataPair[2])));
            }

            return result;
        }

        private string FromUserPairData(List<UserPoint> userdata)
        {
            var sb = new StringBuilder();

            foreach (var pair in userdata)
            {
                sb.Append($"{pair.UserId}|{pair.Balance}|{pair.DailyUsedAt};");
            }

            return sb.ToString();
        }

        private async Task UpdateData(List<UserPoint> userdata)
        {
            await File.WriteAllTextAsync(filePath, FromUserPairData(userdata));
        }
    }

    public class UserPoint
    {
        public UserPoint(string userId, float balance, DateTime dailyUsedAt)
        {
            UserId = userId;
            Balance = balance;
            DailyUsedAt = dailyUsedAt;
        }

        public string UserId { get; set; }
        public float Balance { get; set; }
        public DateTime DailyUsedAt { get; set; }
    }
}
