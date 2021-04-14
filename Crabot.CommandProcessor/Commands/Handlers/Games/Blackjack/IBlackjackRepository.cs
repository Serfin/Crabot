namespace Crabot.Commands.Handlers.Games.Blackjack
{
    public interface IBlackjackRepository
    {
        void AddGame(BlackjackGame game);
        BlackjackGame GetGame(string gameId);
        BlackjackGame GetUserGame(string userId);
        void RemoveGame(string gameId);
    }
}
