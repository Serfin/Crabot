using System;
using System.Threading.Tasks;
using Crabot.Commands.Commands;
using Crabot.Commands.Dispatcher;
using Crabot.Contracts;
using Crabot.Core.Events;
using Newtonsoft.Json;

namespace Crabot.Commands
{
    public class CommandProcessor : ICommandProcessor
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ICommandValidator _commandValidator;

        public CommandProcessor(
            ICommandDispatcher commandDispatcher,
            ICommandValidator commandValidator)
        {
            _commandDispatcher = commandDispatcher;
            _commandValidator = commandValidator;
        }

        public async Task ProcessMessageAsync(GatewayPayload payload)
        {
            var message = JsonConvert.DeserializeObject<Message>(payload.EventData.ToString());

            if (_commandValidator.IsCommand(message))
            {
                var command = new Command(message);

                await _commandDispatcher.DispatchAsync(command);
            }
        }
    }
}
