using System;
using System.Threading.Tasks;
using Autofac;
using Crabot.Core.Repositories;
using Crabot.Rest.RestClient;
using Microsoft.Extensions.Logging;

namespace Crabot.Commands.Dispatcher
{
	public class CommandDispatcher : ICommandDispatcher
	{
		private readonly IComponentContext _component;
		private readonly IDiscordRestClient _discordRestClient;
        private readonly ILogger _logger;

        public CommandDispatcher(
			IComponentContext component, 
			IDiscordRestClient discordRestClient, 
			ILogger<CommandDispatcher> logger)
        {
            _component = component;
            _discordRestClient = discordRestClient;
            _logger = logger;
        }

        public async Task DispatchAsync(Command command)
		{
			try
            {
				if (command.CommandName == "help" && command.Arguments.Count == 1)
                {
					await HandleHelpCommand(command);
					
					return;
				}

				var isRegistered = _component.IsRegisteredWithKey<ICommandHandler>(command.CommandName);
				if (!isRegistered)
				{
					await _component.ResolveKeyed<ICommandHandler>("command-not-found")
						.HandleAsync(command);

					return;
				}

				var commandHandler = _component.ResolveKeyed<ICommandHandler>(command.CommandName);
				if (command.Arguments.Count != commandHandler.GetAttributeCommandArgsCount())
                {
					await DisplayValidationResult(command.CalledFromChannel);
				}
				else
                {
					var validationResult = await commandHandler.ValidateCommandAsync(command);
					if (!validationResult.IsValid)
                    {
						await DisplayValidationResult(command.CalledFromChannel, 
							validationResult.ErrorMessage);
                    }
					else
                    {
						await commandHandler.HandleAsync(command);
					}
				}
            }
			catch (Exception ex)
            {
				_logger.LogError("Error during processing command: {0} \n {1}", ex.Message, ex.StackTrace);
			
				await _component.ResolveKeyed<ICommandHandler>("internal-application-error")
					.HandleAsync(command);
            }
		}

        private async Task HandleHelpCommand(Command command)
        {
			var isRegistered = _component.IsRegisteredWithKey<ICommandHandler>(command.Arguments[0]);
			if (isRegistered)
			{
				var commandUsage = _component.ResolveKeyed<ICommandHandler>(command.Arguments[0])
					.GetCommandUsage();

				if (commandUsage is null)
                {
					await _discordRestClient.PostMessage(command.CalledFromChannel,
						"Help for this command is missing");
                }
				else
                {
					await _discordRestClient.PostMessage(command.CalledFromChannel,
						"Command usage: " + commandUsage);
				}
			}
		}

        public async Task DispatchAsync(Reaction reaction, TrackedMessage trackedMessage)
        {
			try
			{
				var isRegistered = _component.IsRegisteredWithKey<IReactionHandler>(trackedMessage.Command);
				if (!isRegistered)
				{
					throw new ApplicationException($"Requested reaction does not has handler assigned: {trackedMessage.Command}");
				}

				await _component.ResolveKeyed<IReactionHandler>(trackedMessage.Command)
					.HandleAsync(reaction);
			}
			catch (Exception ex)
			{
				_logger.LogError("Error during processing reaction: {0} \n {1}", ex.Message, ex.StackTrace);
			}
		}

		private async Task DisplayValidationResult(string channelId)
		{
			await _discordRestClient.PostMessage(channelId, "Invalid command structure");
		}

		private async Task DisplayValidationResult(string channelId, string commandValidationMessage)
		{
			await _discordRestClient.PostMessage(channelId, commandValidationMessage);
		}
	}
}
