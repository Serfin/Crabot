using System;
using System.Threading.Tasks;
using Autofac;
using Crabot.Commands.Dispatcher;
using Microsoft.Extensions.Logging;

namespace Crabot.Commands.Dispatcher
{
	public class CommandDispatcher : ICommandDispatcher
	{
		private readonly IComponentContext _component;
        private readonly ILogger _logger;

        public CommandDispatcher(
			IComponentContext component, 
			ILogger<CommandDispatcher> logger)
        {
            _component = component;
            _logger = logger;
        }

        public async Task DispatchAsync(Command command)
		{
			var isRegistered = _component.IsRegisteredWithKey<ICommandHandler>(command.CommandName);

			if (!isRegistered)
            {
				await _component.ResolveKeyed<ICommandHandler>("error").HandleAsync(command);

				return;
			}

			var commandHandler = _component.ResolveKeyed<ICommandHandler>(command.CommandName);

			try
            {
				await commandHandler.HandleAsync(command);
            }
			catch (Exception ex)
            {
				_logger.LogError("Error during processing command: {0} \n {1}", ex.Message, ex.StackTrace);
			
				await _component.ResolveKeyed<ICommandHandler>("error").HandleAsync(command);
            }
		}
	}
}
