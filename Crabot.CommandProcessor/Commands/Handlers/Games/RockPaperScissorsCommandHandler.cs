﻿using System;
using System.Threading.Tasks;
using Crabot.Commands.Dispatcher;
using Crabot.Core.Repositories;
using Crabot.Rest.Models;
using Crabot.Rest.RestClient;

namespace Crabot.Commands.Commands.Handlers.Games
{
    [Command("rps")] // ?rps <user> <points>
    public class RockPaperScissorsCommandHandler : ICommandHandler
    {
        private readonly IUserPointsRepository _userPointsRepository;
        private readonly IDiscordRestClient _discordRestClient;

        private Command _command;
        private float gameBounty;
        private string[] possibleSymbols = { ":rock:", ":roll_of_paper:", ":scissors:" };

        public RockPaperScissorsCommandHandler(
            IUserPointsRepository userPointsRepository, 
            IDiscordRestClient discordRestClient)
        {
            _userPointsRepository = userPointsRepository;
            _discordRestClient = discordRestClient;
        }

        public async Task HandleAsync(Command command)
        {
            _command = command;

            var (isValid, errorMessage) = await ValidateArguments(_command);
            if (!isValid)
            {
                await _discordRestClient.PostMessage(_command.CalledFromChannel,
                   new Message { Content =  errorMessage});

                return;
            }

            await PlayGame();
        }

        private async Task<(bool isValid, string errorMessage)> ValidateArguments(Command command)
        {
            if (command.Arguments.Count != 2)
            {
                return (false, $"Invalid amount of arguments. Command requires 2 argument ?rps <user> <amount>");
            }

            if (!float.TryParse(command.Arguments[1], out float requiredBalance))
            {
                return (false, $"Invalid command structure - rpc <user> <points_to_bet>");
            }

            gameBounty = requiredBalance;
            //var userCurrentBalance = await _userPointsRepository.GetUserBalanceAsync(command.Author.Id);
            //if (userCurrentBalance < requiredBalance)
            //{
            //    return (false, $"User **{command.Author.Username}** does not have enough points");
            //} 
            //else
            //{
            //    userBalance = userCurrentBalance;
            //}

            return (true, string.Empty);
        }

        private async Task PlayGame()
        {
            var rnd = new Random();
            var attackerRoll = rnd.Next(0, 3);
            var defenderRoll = rnd.Next(0, 3);

            var attackerWon = false;

            if (attackerRoll == defenderRoll)
            {
                await _discordRestClient.PostMessage(_command.CalledFromChannel,
                   new Message { Content = "Draw! No points" });

                return;
            }
            else if (attackerRoll == 0 && defenderRoll == 2) // Rock vs Scissors
            {
                attackerWon = true;
            }
            else if (attackerRoll == 0 && defenderRoll == 1) // Rock vs Paper
            {
                attackerWon = false;
            }
            else if (attackerRoll == 2 && defenderRoll == 1) // Scissors vs Paper
            {
                attackerWon = true;
            }
            else if (attackerRoll == 2 && defenderRoll == 0) // Scissors vs Rock
            {
                attackerWon = false;
            }
            else if (attackerRoll == 1 && defenderRoll == 0) // Paper vs Rock
            {
                attackerWon = true;
            }
            else if (attackerRoll == 1 && defenderRoll == 2) // Paper vs Scissors
            {
                attackerWon = false;
            }

            await DisplayGameResult(attackerWon, attackerRoll, defenderRoll);
        }

        private async Task DisplayGameResult(bool attackerWon, int attackerRoll, int defenderRoll)
        {
            if (attackerWon)
            {
                await _discordRestClient.PostMessage(_command.CalledFromChannel,
                    new Message
                    {
                        Content = $"**{_command.Author.Username}** won **{gameBounty * 2}** points! " +
                                  $"[**{_command.Author.Username}** - {possibleSymbols[attackerRoll]} | " +
                                  $"**{_command.Arguments[0]}** - {possibleSymbols[defenderRoll]}]"
                    });
            }
            else
            {
                await _discordRestClient.PostMessage(_command.CalledFromChannel,
                    new Message
                    {
                        Content = $"**{_command.Author.Username}** lost **{gameBounty}**  points! " +
                                  $"[**{_command.Author.Username}** - {possibleSymbols[attackerRoll]} | " +
                                  $"**{_command.Arguments[0]}** - {possibleSymbols[defenderRoll]}]"
                    });
            }
        }
    }
}