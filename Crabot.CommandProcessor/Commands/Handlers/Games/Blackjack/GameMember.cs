using System.Collections.Generic;
using System.Linq;

namespace Crabot.Commands.Handlers.Games.Blackjack
{
    public class GameMember
    {
        public string UserId { get; private set; }
        public string UserNickname { get; private set; }
        public List<GameCard> UserCards { get; private set; }
        public bool UserMadeChoice { get; set; }
        public bool UserStands { get; set; }
        public int CurrentScore => UserCards.Sum(x => x.Weight);

        public GameMember(string userId, string userNickname)
        {
            UserId = userId;
            UserNickname = userNickname;
            UserCards = new List<GameCard>();
            UserMadeChoice = false;
            UserStands = false;
        }
    }
}
