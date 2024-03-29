﻿using Crabot.Contracts;
using Crabot.Core.Repositories;

namespace Crabot.Commands.Commands
{
    public class CommandValidator : ICommandValidator
    {
        private readonly IClientInfoRepository _clientInfoRepository;

        public CommandValidator(IClientInfoRepository clientInfoRepository)
        {
            _clientInfoRepository = clientInfoRepository;
        }

        public bool IsCommand(GatewayMessage message)
        {
            return message.Content.Length > 0
                && message.Content.TrimStart()[0] == '?'
                && !IsBotAnAuthor(message.Author.Id);
        }

        public bool IsBotAnAuthor(string userId)
        {
            return userId == _clientInfoRepository.GetClientInfo().User.Id;
        }
    }
}
