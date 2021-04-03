using Crabot.Commands.Commands;
using Crabot.Contracts;
using Crabot.Core.Repositories;
using Moq;
using NUnit.Framework;

namespace Crabot.Tests
{
    [TestFixture]
    public class CommandValidatorTests
    {
        private Mock<IClientInfoRepository> _clientInfoReposotoryMock;
        private CommandValidator _commandValidator;

        private string crabotFakeAuthorId = "112233445566778899";

        [OneTimeSetUp]
        public void Setup()
        {
            _clientInfoReposotoryMock = new Mock<IClientInfoRepository>();
            _clientInfoReposotoryMock
                .Setup(x => x.GetClientInfo())
                .Returns(new ClientInfo
                {
                    User = new User { Id = crabotFakeAuthorId }
                });

            _commandValidator = new CommandValidator(_clientInfoReposotoryMock.Object);
        }

        [Test]
        public void Command_with_prefix_and_random_author_id_is_valid_command()
        {
            var testCommand = new GatewayMessage 
            { 
                Content = "?test-command", 
                Author = new Author 
                { 
                    Id = It.IsAny<string>() 
                } 
            };

            var validationResult = _commandValidator.IsCommand(testCommand);

            Assert.True(validationResult);
        }

        [Test]
        public void Bot_cannot_handle_its_own_commands()
        {
            var testCommand = new GatewayMessage
            {
                Content = "?test-command",
                Author = new Author
                {
                    Id = crabotFakeAuthorId
                }
            };

            var validationResult = _commandValidator.IsCommand(testCommand);

            Assert.False(validationResult);
        }
    }
}
