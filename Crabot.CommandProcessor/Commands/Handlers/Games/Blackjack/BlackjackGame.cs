using System;
using System.Collections.Generic;
using System.Linq;

namespace Crabot.Commands.Handlers.Games.Blackjack
{
    public class BlackjackGame
    {
        public string Id { get; private set; }
        public string GameOwner { get; private set; }
        public float MoneyToWin => Members.Count * BetAmount;
        public float BetAmount { get; private set; }
        public bool IsInProgress { get; private set; }
        public List<GameMember> Members { get; private set; }
        public List<GameCard> GameDeck { get; private set; }

        private object GameDeckLocker = new object();

        public BlackjackGame(string id, string gameOwner, float betAmount)
        {
            Id = id;
            GameOwner = gameOwner;
            BetAmount = betAmount;
            Members = new List<GameMember>();
            IsInProgress = false;
            GameDeck = GenerateDeck();
        }

        public void StartGame()
        {
            IsInProgress = true;

            foreach (var member in Members)
            {
                DrawCardForUser(member.UserId);
            }
        }

        public void EndGame()
        {
            IsInProgress = false;
        }

        public void NewTurn()
        {
            foreach (var member in Members)
            {
                if (!member.UserStands)
                {
                    member.UserMadeChoice = false;
                }
            }
        }

        public IEnumerable<string> CheckWinner()
        {
            // All players above 21
            if (this.Members.All(x => x.UserStands) && this.Members.All(x => x.CurrentScore > 21))
            {
                EndGame();
                return new List<string> { "no-winners" };
            }

            // All player stands
            if (this.Members.All(x => x.UserStands))
            {
                // Take users which are closest to 21 but not above 21
                EndGame();
                return GetPlayersClosestTo21(this.Members);
            }

            // At least one of players has 21
            var users = this.Members.Where(x => x.CurrentScore == 21);
            if (users.Count() > 0)
            {
                EndGame();
                return users.Select(x => x.UserId);
            }

            // At least on of users has above 21 
            users = this.Members.Where(x => x.CurrentScore > 21);
            if (users.Count() > 0)
            {
                EndGame();
                return GetPlayersClosestTo21(this.Members);
            }

            return null;
        }

        public void AddMember(string userId, string userNickname)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("User id cannot be null or empty");
            }

            if (string.IsNullOrEmpty(userNickname))
            {
                throw new ArgumentNullException("User nickname cannot be null or empty");
            }

            this.Members.Add(new GameMember(userId, userNickname));
        }

        public void RemoveMember(string userId)
        {
            var member = this.Members.Find(x => x.UserId == userId);

            if (member is null)
            {
                throw new ArgumentNullException("User with given id does not participate in game");
            }

            this.Members.Remove(member);
        }

        public void DrawCardForUser(string userId)
        {
            lock (GameDeckLocker)
            {
                var user = this.Members.Where(x => x.UserId == userId).First();

                var drawnCard = GameDeck.First();
                user.UserCards.Add(drawnCard);
                GameDeck.Remove(drawnCard);
            }
        }

        private List<GameCard> GenerateDeck()
        {
            var gameDeck = new List<GameCard>();
            var weights = new (string Symbol, int Weight)[] 
            { 
                ("A", 11), ("K", 10), ("Q", 10), ("J", 10),
                ("10", 10), ("9", 9), ("8", 8), ("7", 7),
                ("6", 6), ("5", 5), ("4", 4), ("3", 3), ("2", 2)
            };
            
            var colors = new string[] { "Spade", "Heart", "Club", "Diamond" };

            foreach (var pair in weights)
            {
                foreach (var color in colors)
                {
                    gameDeck.Add(new GameCard(pair.Weight, pair.Symbol, color));
                }
            }

            Shuffle(gameDeck);

            return gameDeck;
        }

        private void Shuffle(List<GameCard> list)
        {
            var rng = new Random();

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                GameCard value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    
        private IEnumerable<string> GetPlayersClosestTo21(IEnumerable<GameMember> source)
        {
            var usersBelow21 = source.Where(x => x.CurrentScore < 21);

            if (usersBelow21.Count() < 1)
            {
                return new List<string> { "no-winners" };
            }

            return usersBelow21
                .OrderByDescending(x => x.CurrentScore)
                .GroupBy(x => x.CurrentScore)
                .First()
                .Select(x => x.UserId);
        }
    }
}
