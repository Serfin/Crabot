﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Crabot.Commands.Dispatcher;
using Crabot.Core.Repositories;
using Crabot.Rest.Models;
using Crabot.Rest.RestClient;

namespace Crabot.Commands.Commands.Handlers.Games
{
    [Command("slots", 1)]
    [CommandUsage("?slots <amount-to-bet>")]
    public class SlotsCommandHandler : ICommandHandler
    {
        private readonly IUserPointsRepository _userPointsRepository;
        private readonly IDiscordRestClient _discordRestClient;

        private Command _command;
        private float gameBounty;
        private float gameWeightMultiplier;

        private readonly (string Symbol, float Weight)[] Barrel = 
            new (string, float)[]
        {
            (":apple:", 1.4f),
            (":tangerine:", 1.4f),
            (":lemon:", 1.5f),
            (":watermelon:", 1.6f),
            (":strawberry:", 2.0f),
            (":melon:", 2.5f),
            (":cherries:", 3f),
            (":peach:", 5f),
            (":kiwi:", 7f),
            (":banana:", 10f)
        };

        public SlotsCommandHandler(
            IUserPointsRepository userPointsRepository,
            IDiscordRestClient discordRestClient)
        {
            _userPointsRepository = userPointsRepository;
            _discordRestClient = discordRestClient;
        }

        public async Task HandleAsync(Command command)
        {
            var commandValidation = ValidateCommandStructure(command);
            if (!commandValidation.Result)
            {
                await _discordRestClient.PostMessage(command.CalledFromChannel,
                    new Message { Content = commandValidation.ValidationMessage });

                return;
            }

            _command = command;

            var userBalance = await _userPointsRepository.GetUserBalanceAsync(command.Author.Id);

            var validation = ValidateUser(userBalance, gameBounty);
            if (!validation.Result)
            {
                await _discordRestClient.PostMessage(command.CalledFromChannel,
                    new Message { Content = validation.ValidationMessage });

                return;
            }

            await PlayGame();
        }

        private async Task PlayGame()
        {
            var rnd = new Random();

            var roll1 = rnd.Next(0, Barrel.Length);
            var roll2 = rnd.Next(0, Barrel.Length);
            var roll3 = rnd.Next(0, Barrel.Length);

            var gameWon = false;
            if (roll1 == roll2 && roll1 == roll3)
            {
                gameWon = true;
                gameWeightMultiplier = Barrel[roll1].Weight * 3;
            } 
            else if (roll1 == roll2 || roll1 == roll3 || roll2 == roll3)
            {
                gameWon = true;

                var rolls = new int[] { roll1, roll2, roll3 };

                gameWeightMultiplier = Barrel[rolls.GroupBy(x => x).First(x => x.Count() > 1).First()].Weight;
            }

            await DisplayResult(roll1, roll2, roll3, gameWon);
            await ManagePoints(gameWon);
        }

        private async Task ManagePoints(bool gameWon)
        {
            if (gameWon)
            {
                await _userPointsRepository.AddBalanceToUserAccount(_command.Author.Id, 
                    (gameBounty * gameWeightMultiplier) - gameBounty);
            }
            else
            {
                await _userPointsRepository.SubtractBalanceFromUserAccount(_command.Author.Id, gameBounty);
            }
        }

        private async Task DisplayResult(int roll1, int roll2, int roll3, bool gameWon)
        {
            var stringResult = string.Empty;

            stringResult += "----- **[SLOTS]** -----\n";
            stringResult += "----------------------\n";
            stringResult += $"[{GetBarrelSymbol(roll1 - 1)}] | [{GetBarrelSymbol(roll2 - 1)}] | [{GetBarrelSymbol(roll3 - 1)}] \n";
            stringResult += $"**[{GetBarrelSymbol(roll1)}] | [{GetBarrelSymbol(roll2)}] | [{GetBarrelSymbol(roll3)}] < ** \n";
            stringResult += $"[{GetBarrelSymbol(roll1 + 1)}] | [{GetBarrelSymbol(roll2 + 1)}] | [{GetBarrelSymbol(roll3 + 1)}]\n";
            stringResult += "----------------------\n";


            if (gameWon)
            {
                stringResult += $"**| : : : : :  WIN : : : : : |**  \n\n **{_command.Author.Username}** used **{_command.Arguments[0]}** credit(s) and won **{gameBounty * gameWeightMultiplier:0.##}** credits!";
            }
            else
            {
                stringResult += $"**|: : : : :  LOST : : : : :|**  \n\n **{_command.Author.Username}** used **{gameBounty}** credit(s) and lost everything!";
            }


            await _discordRestClient.PostMessage(_command.CalledFromChannel,
                new Message { Content = stringResult });
        }

        private string GetBarrelSymbol(int n)
        {
            if (n > Barrel.Length - 1)
            {
                return Barrel[0].Symbol;
            }

            if (n < 0)
            {
                return Barrel[Barrel.Length - 1].Symbol;
            }

            return Barrel[n].Symbol;
        }

        private (bool Result, string ValidationMessage) ValidateCommandStructure(Command command)
        {
            if (command.Arguments.Count != 1)
            {
                return (false, "Invalid command structure - ?slots <amount_to_bet>");
            }

            var gameBetParseResult = float.TryParse(command.Arguments[0], out float gameBet);

            if (gameBetParseResult)
            {
                if (gameBet <= 0)
                {
                    return (false, $"Invalid bet amount");
                }
            }
            else
            {
                return (false, "Invalid bet amount - ?slots <amount_to_bet>");
            }

            gameBounty = gameBet;

            return (true, string.Empty);
        }

        private (bool Result, string ValidationMessage) ValidateUser(float? balance, float requiredBalance)
        {
            if (balance is null)
            {
                return (false, "User does not own a wallet, use ?regiser to register new wallet");
            }

            if (balance.Value < requiredBalance)
            {
                return (false, "User does not have enough points to play");
            }

            return (true, string.Empty);
        }

        public async Task<ValidationResult> ValidateCommandAsync(Command command)
        {
            return await Task.FromResult(new ValidationResult(true));
        }
    }
}
