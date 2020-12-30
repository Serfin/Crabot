using System;
using System.Threading.Tasks;
using Crabot.Commands.Commands.Models;
using Crabot.Commands.Models;
using Crabot.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Crabot.Commands.Dispatcher
{
	public class CommandDispatcher : ICommandDispatcher
	{
		private readonly IServiceProvider _serviceProvider;

		public CommandDispatcher(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public async Task DispatchAsync(Message message)
		{
			var commandName = message.Content.ToLower()[1..];

			switch (commandName)
            {
				case "ping":
					await _serviceProvider.GetService<ICommandHandler<PingCommand>>()
						.HandleAsync(new PingCommand(message));
					break;
				case "champ":
					await _serviceProvider.GetService<ICommandHandler<ChampCommand>>()
						.HandleAsync(new ChampCommand(message));
					break;
				default:
					await _serviceProvider.GetService<ICommandHandler<CommandError>>()
						.HandleAsync(new CommandError(message));
					break;
            }
		}
	}
}
