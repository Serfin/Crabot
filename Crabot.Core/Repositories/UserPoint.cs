using System;

namespace Crabot.Core.Repositories
{
    public class UserPoint
    {
        public UserPoint(string nickname, string userId, float balance, DateTime dailyUsedAt)
        {
            Nickname = nickname;
            UserId = userId;
            Balance = balance;
            DailyUsedAt = dailyUsedAt;
        }

        public string Nickname { get; set; }
        public string UserId { get; set; }
        public float Balance { get; set; }
        public DateTime DailyUsedAt { get; set; }
    }
}
