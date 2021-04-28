using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crabot.Commands.Commands;
using Crabot.Commands.Dispatcher;
using Crabot.Commands.Handlers.Games.Blackjack;
using Crabot.Contracts;
using Crabot.Core.Repositories;
using Crabot.MessageExtensions;
using Crabot.Rest.Models;
using Crabot.Rest.RestClient;

namespace Crabot.Commands.Handlers.Games
{
    [Command("blackjack", 1)]
    [CommandUsage("?blackjack <bet-amount>")]
    public class BlackjackCommandHandler : ICommandHandler, IReactionHandler 
    {
        private readonly IDiscordRestClient _discordRestClient;
        private readonly IGuildRepository _guildRepository;
        private readonly IUserPointsRepository _usersPointsRepository;
        private readonly IBlackjackRepository _blackjackRepository;
        private readonly ITrackedMessageRepository _trackedMessageRepository;

        private Emoji joinEmoji, startEmoji, hitEmoji, standEmoji;

        public BlackjackCommandHandler(
            IDiscordRestClient discordRestClient, 
            IGuildRepository guildRepository,
            IUserPointsRepository usersPointsRepository,
            IBlackjackRepository blackjackRepository,
            ITrackedMessageRepository trackedMessageRepository)
        {
            _discordRestClient = discordRestClient;
            _guildRepository = guildRepository;
            _usersPointsRepository = usersPointsRepository;
            _blackjackRepository = blackjackRepository;
            _trackedMessageRepository = trackedMessageRepository;
        }

        public async Task HandleAsync(Command command)
        {
            joinEmoji = GetEmoji(command.CalledFromGuild, "Join");
            startEmoji = GetEmoji(command.CalledFromGuild, "Stonks");

            var response = await _discordRestClient.PostMessage(command.CalledFromChannel,
                $"Created blackjack game! Use {MessageUtilities.UseEmoji(joinEmoji)} to join game, " +
                $"{MessageUtilities.MentionUser(command.Author.Id)} use {MessageUtilities.UseEmoji(startEmoji)} to start game!");

            await _discordRestClient.AddReactionToMessage(command.CalledFromChannel,
                response.Data.Id, new Emoji[] { joinEmoji, startEmoji });

            await _trackedMessageRepository.AddTrackedMessageAsync(new TrackedMessage(command.CalledFromGuild, 
                command.CalledFromChannel, response.Data.Id, this.GetAttributeCommandName(),
                new Dictionary<string, string> { { joinEmoji.Name, joinEmoji.Id } }));

            _blackjackRepository.AddGame(new BlackjackGame(response.Data.Id, 
                command.Author.Id, float.Parse(command.Arguments[0])));
        }

        public async Task HandleAsync(Reaction reaction)
        {
            joinEmoji = GetEmoji(reaction.GuildId, "Join");
            startEmoji = GetEmoji(reaction.GuildId, "Stonks");
            hitEmoji = GetEmoji(reaction.GuildId, "PogU");
            standEmoji = GetEmoji(reaction.GuildId, "PogO");

            if (reaction.Emoji.Id == joinEmoji.Id)
            {
                await HandleJoinReaction(reaction);
            }
            else if (reaction.Emoji.Id == startEmoji.Id && reaction.IsAdded)
            {
                await HandleStartReaction(reaction);
            }
            else if (reaction.Emoji.Name == hitEmoji.Name && reaction.IsAdded)
            {
                await HandleHitReaction(reaction);
            }
            else if (reaction.Emoji.Name == standEmoji.Name && reaction.IsAdded)
            {
                await HandleStandReaction(reaction);
            }
        }

        private async Task HandleJoinReaction(Reaction reaction)
        {
            if (reaction.IsAdded)
            {
                var game = _blackjackRepository.GetGame(reaction.MessageId);
                if (!game.IsInProgress)
                {
                    var userBalance = await _usersPointsRepository.GetUserBalanceAsync(reaction.UserId);
                    userBalance = 100000; // DEV
                    if (userBalance.Value >= game.BetAmount)
                    {
                        game.AddMember(reaction.UserId, reaction.Member.User.Username);

                        await _discordRestClient.PostMessage(reaction.ChannelId,
                            $"User {MessageUtilities.MentionUser(reaction.UserId)} joined the game");
                    }
                    else
                    {
                        await _discordRestClient.PostMessage(reaction.ChannelId,
                            $"User {MessageUtilities.MentionUser(reaction.UserId)} " +
                            "does not have enough points to join the game");
                    }
                }
                else
                {
                    await _discordRestClient.PostMessage(reaction.ChannelId,
                        $"{MessageUtilities.MentionUser(reaction.UserId)} game already started");
                }
            }
            else
            {
                var game = _blackjackRepository.GetGame(reaction.MessageId);
                if (!game.IsInProgress)
                {
                    game.RemoveMember(reaction.MessageId);

                    await _discordRestClient.PostMessage(reaction.ChannelId,
                        $"User {MessageUtilities.MentionUser(reaction.UserId)} leaved the game");
                }
            }
        }

        private async Task HandleStartReaction(Reaction reaction)
        {
            var game = _blackjackRepository.GetGame(reaction.MessageId);

            if (!game.IsInProgress && reaction.UserId == game.GameOwner && game.Members.Count > 1)
            {
                game.StartGame();

                var messageId = await DisplayGameStatus(reaction.ChannelId, game);

                await _trackedMessageRepository.AddTrackedMessageAsync(new TrackedMessage
                    (reaction.GuildId, reaction.ChannelId, messageId,
                    this.GetAttributeCommandName(), new Dictionary<string, string> {
                            { hitEmoji.Name, hitEmoji.Id },
                            { standEmoji.Name, standEmoji.Id }}));

                await _discordRestClient.AddReactionToMessage(reaction.ChannelId,
                    messageId, new Emoji[] { hitEmoji, standEmoji });
            }
        }

        private async Task HandleHitReaction(Reaction reaction)
        {
            var game = _blackjackRepository.GetUserGame(reaction.UserId);
            var member = game.Members.FirstOrDefault(x => x.UserId == reaction.UserId);

            if (member != null && member.CurrentScore < 21 && !member.UserStands)
            {
                member.UserMadeChoice = true;
                game.DrawCardForUser(member.UserId);
            }

            if (game.Members.All(x => x.UserMadeChoice))
            {
                var winners = game.CheckWinner();
                if (winners?.Count() > 0)
                {
                    await DisplayGameStatus(reaction.ChannelId, game);
                    await DisplayWinners(reaction.ChannelId, winners);
                    await ManagePoints(winners, game);

                    _blackjackRepository.RemoveGame(game.Id);
                    await _trackedMessageRepository.RemoveTrackedMessageAsync(
                        reaction.ChannelId, reaction.MessageId);

                    return;
                }

                var messageId = await DisplayGameStatus(reaction.ChannelId, game);
                game.NewTurn();

                await _trackedMessageRepository.AddTrackedMessageAsync(new TrackedMessage
                    (reaction.GuildId, reaction.ChannelId, messageId,
                    this.GetAttributeCommandName(), new Dictionary<string, string> {
                            { hitEmoji.Name, hitEmoji.Id },
                            { standEmoji.Name, standEmoji.Id }}));

                await _discordRestClient.AddReactionToMessage(reaction.ChannelId,
                   messageId, new Emoji[] { hitEmoji, standEmoji });
            }
        }

        private async Task HandleStandReaction(Reaction reaction)
        {
            var game = _blackjackRepository.GetUserGame(reaction.UserId);
            var member = game.Members.FirstOrDefault(x => x.UserId == reaction.UserId);
            
            if (member != null && !member.UserMadeChoice)
            {
                member.UserMadeChoice = true;
                member.UserStands = true;
            }

            if (game.Members.All(x => x.UserMadeChoice))
            {
                var winners = game.CheckWinner();
                if (winners?.Count() > 0)
                {
                    await DisplayGameStatus(reaction.ChannelId, game);
                    await DisplayWinners(reaction.ChannelId, winners);
                    await ManagePoints(winners, game);

                    _blackjackRepository.RemoveGame(game.Id);
                    await _trackedMessageRepository.RemoveTrackedMessageAsync(
                        reaction.ChannelId, reaction.MessageId);

                    return;
                }

                var messageId = await DisplayGameStatus(reaction.ChannelId, game);
                game.NewTurn();

                await _trackedMessageRepository.AddTrackedMessageAsync(new TrackedMessage
                    (reaction.GuildId, reaction.ChannelId, messageId,
                    this.GetAttributeCommandName(), new Dictionary<string, string> {
                            { hitEmoji.Name, hitEmoji.Id },
                            { standEmoji.Name, standEmoji.Id }}));

                await _discordRestClient.AddReactionToMessage(reaction.ChannelId,
                    messageId, hitEmoji);
                await _discordRestClient.AddReactionToMessage(reaction.ChannelId,
                    messageId, standEmoji);
            }
        }

        private async Task ManagePoints(IEnumerable<string> winners, BlackjackGame game)
        {
            // Add points to winners
            if (winners.Count() == 1)
            {
                await _usersPointsRepository.AddBalanceToUserAccount(winners.First(), game.MoneyToWin);
            }
            else
            {
                var splittedAmount = Math.Floor(game.MoneyToWin / winners.Count());
                foreach (var winner in winners)
                {
                    await _usersPointsRepository.AddBalanceToUserAccount(winner, (float)splittedAmount);
                }
            }

            // Remove points from loosers
            var losers = game.Members.Where(x => !winners.ToList().Contains(x.UserId))
                .Select(x => x.UserId);
            foreach (var loser in losers)
            {
                await _usersPointsRepository.SubtractBalanceFromUserAccount(loser, game.BetAmount);
            }
        }

        private async Task DisplayWinners(string channelId, IEnumerable<string> winners)
        {
            if (winners.Contains("no-winners"))
            {
                await _discordRestClient.PostMessage(channelId, 
                    $"No winners");

                return;
            }

            if (winners.Count() > 1)
            {
                var userIds = "Winners ";
                foreach (var winner in winners)
                {
                    userIds += $"{MessageUtilities.MentionUser(winner)} ";
                }

                await _discordRestClient.PostMessage(channelId, userIds);
            }
            else
            {
                var winner = winners.First();

                await _discordRestClient.PostMessage(channelId, $"Winner {MessageUtilities.MentionUser(winner)}");
            }
        }

        private async Task<string> DisplayGameStatus(string channelId, BlackjackGame game)
        {
            var messageBuilder = new EbmedMessageBuilder()
                    .AddCustomAuthor("Blackjack");

            messageBuilder.AddMessageField(new EmbedField("Prize pool", 
                string.Format("{0:0.##}", game.MoneyToWin)));

            foreach (var member in game.Members)
            {
                var userCards = string.Empty;

                foreach (var card in member.UserCards)
                {
                    userCards += $"``{card.Symbol} {GetCardEmoji(card.Color)}`` ";
                }

                userCards += $"({string.Format(TextFormat.BoldText, member.CurrentScore)})";

                messageBuilder.AddMessageField(new EmbedField(
                    string.Format(TextFormat.BoldText, member.UserNickname), userCards));
            }

            var response = await _discordRestClient.PostMessage(channelId, 
                new Message { Embed = messageBuilder.Build() });
            
            return response.Data.Id;
        }
    
        private string GetCardEmoji(string cardColor)
        {
            return cardColor switch
            {
                "Spade" => "♠",
                "Heart" => "♥",
                "Club" => "♣",
                "Diamond" => "♦",
                _ => ""
            }; 
        }

        private Emoji GetEmoji(string guildId, string emojiName, bool isCustomEmoji = true)
        {
            if (isCustomEmoji)
            {
                return _guildRepository.GetGuild(guildId)?
                    .Emojis.FirstOrDefault(x => x.Name == emojiName);
            }

            return new Emoji { Name = emojiName, Id = "" };
        }

        public async Task<ValidationResult> ValidateCommandAsync(Command command)
        {
            if (command.Arguments.Count != 1)
            {
                return new ValidationResult(false, "Invalid amount of arguments - ?blackjack <amount_to_bet>");
            }

            if (!float.TryParse(command.Arguments[0], out float gameBounty) || gameBounty <= 0)
            {
                return new ValidationResult(false, "Invalid argument - ?blackjack <amount_to_bet>");
            }

            var userBalance = await _usersPointsRepository.GetUserBalanceAsync(command.Author.Id);
            if (userBalance < gameBounty)
            {
                return new ValidationResult(false, "You don't have enough points to start gane - ?blackjack <amount_to_bet>");
            }

            return new ValidationResult(true, null);
        }
    }
}