using System;
using System.Threading.Tasks;
using Autofac;
using Crabot.Commands.Commands.Models;

namespace Crabot.Commands.Dispatcher
{
	public class CommandDispatcher : ICommandDispatcher
	{
		private readonly IComponentContext _component;

        public CommandDispatcher(IComponentContext component)
        {
            _component = component;
        }

        public async Task DispatchAsync(Command command)
		{
			var commandHandler = _component.ResolveKeyed<ICommandHandler>(command.CommandName);

			if (commandHandler is null)
            {
				throw new ArgumentNullException("Specified command does not have assigned handler!");
            }

			try
            {
				await commandHandler.HandleAsync(command);
            }
			catch (Exception ex)
            {
				// TODO: use dedicated logger
				Console.WriteLine(ex.Message);
            }
		}
	}
}
