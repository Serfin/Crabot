using System.Threading.Tasks;
using Crabot.Commands.Dispatcher;
using Crabot.Contracts;
using Crabot.Repositories;
using Newtonsoft.Json;

namespace Crabot.EventHandlers
{
    public class MessageCreatedEventHandler : IGatewayEventHandler<MessageCreatedEvent>
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IClientInfoRepository _clientInfoRepository;

        public MessageCreatedEventHandler(ICommandDispatcher commandDispatcher, 
            IClientInfoRepository clientInfoRepository)
        {
            _commandDispatcher = commandDispatcher;
            _clientInfoRepository = clientInfoRepository;
        }

        public async Task HandleAsync(object @event)
        {
            var message = JsonConvert.DeserializeObject<Message>(@event.ToString());

            if (ValidateCommand(message))
            {
                _ = Task.Factory.StartNew(async () => await _commandDispatcher.DispatchAsync(message));

                await Task.CompletedTask;
            }
        }

        private bool ValidateCommand(Message message)
        {
            return message.Content.Length > 0
                && message.Content.TrimStart()[0] == '?'
                && message.Author.Id != _clientInfoRepository.GetClientInfo().User.Id;
        }
    }
}
