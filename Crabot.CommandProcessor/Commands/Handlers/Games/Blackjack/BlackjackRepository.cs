using System;
using System.Collections.Generic;
using System.Linq;

namespace Crabot.Commands.Handlers.Games.Blackjack
{
    public class BlackjackRepository : IBlackjackRepository
    {
        public List<BlackjackGame> Games { get; private set; }

        public BlackjackRepository()
        {
            Games = new List<BlackjackGame>();
        }

        public void AddGame(BlackjackGame game)
        {
            if (game is null)
            {
                throw new ArgumentNullException($"Argument {nameof(game)} cannot be null");
            }

            this.Games.Add(game);
        }

        public BlackjackGame GetGame(string gameId)
        {
            return this.Games.Where(x => x.Id == gameId).FirstOrDefault();
        }

        public BlackjackGame GetUserGame(string userId)
        {
            return this.Games.Where(x => x.Members.Any(x => x.UserId == userId)).FirstOrDefault();
        }

        public void RemoveGame(string gameId)
        {
            var game = GetGame(gameId);
            if (game is null)
            {
                throw new ArgumentNullException($"Game with id: {gameId} does not exist");
            }

            this.Games.Remove(game);
        }
    }
}
