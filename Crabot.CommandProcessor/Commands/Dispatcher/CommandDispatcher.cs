using System;
using System.Threading.Tasks;
using Autofac;
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
			try
            {
				var isRegistered = _component.IsRegisteredWithKey<ICommandHandler>(command.CommandName);
				if (!isRegistered)
				{
					await _component.ResolveKeyed<ICommandHandler>("command-not-found")
						.HandleAsync(command);

					return;
				}

				await _component.ResolveKeyed<ICommandHandler>(command.CommandName)
					.HandleAsync(command);
            }
			catch (Exception ex)
            {
				_logger.LogError("Error during processing command: {0} \n {1}", ex.Message, ex.StackTrace);
			
				await _component.ResolveKeyed<ICommandHandler>("internal-application-error")
					.HandleAsync(command);
            }
		}

        public Task DispatchAsync(Reaction reaction)
        {
            throw new NotImplementedException();
        }
    }
}
