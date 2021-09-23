using System;
using System.Threading.Tasks;
using Crabot.Commands.Dispatcher;
using Crabot.Voice;

namespace Crabot.Commands.Commands.Handlers
{
    [Command("voice", 0)]
    public class VoiceCommandHandler : ICommandHandler
    {
        private readonly VoiceClient _voiceClient;
        
        public VoiceCommandHandler(VoiceClient voiceClient)
        {
            _voiceClient = voiceClient;
        }

        public async Task HandleAsync(Command command)
        {
            //await _voiceClient.JoinChannel(command.CalledFromGuild,
            //    "689470875623489549");
        }

        public Task<ValidationResult> ValidateCommandAsync(Command command)
        {
            return Task.FromResult(new ValidationResult(true));
        }
    }
}
