namespace Crabot.Commands.Handlers.Games.Blackjack
{
    public struct GameCard
    {
        public int Weight { get; private set; }
        public string Symbol { get; private set; }
        public string Color { get; private set; }

        public GameCard(int weight, string symbol, string color)
        {
            Weight = weight;
            Symbol = symbol;
            Color = color;
        }
    }
}
