using Crabot.Commands;
using Crabot.Commands.Commands;
using Crabot.Commands.Dispatcher;
using Crabot.Contracts;
using Crabot.Core.Events;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Crabot.Tests
{
    [TestFixture]
    public class CommandProcesorTests
    {
        private Mock<ICommandDispatcher> commandDispatcherMock;
        private Mock<ICommandValidator> commandValidatorMock;
        private CommandProcessor commandProcessor;

        private GatewayPayload testPayload = new GatewayPayload
        {
            EventData = JsonConvert.SerializeObject(
                new GatewayMessage { Content = "?test-command" })
        };

        [SetUp]
        public void Setup()
        {
            commandDispatcherMock = new Mock<ICommandDispatcher>();
            commandValidatorMock = new Mock<ICommandValidator>();
            commandProcessor = new CommandProcessor(commandDispatcherMock.Object,
                commandValidatorMock.Object);
        }

        [Test]
        public void Valid_command_is_dispatched()
        {
            commandValidatorMock
                .Setup(x => x.IsCommand(It.IsAny<GatewayMessage>()))
                .Returns(true);

            commandProcessor.ProcessMessageAsync(testPayload)
                .Wait();

            commandDispatcherMock.Verify(x => 
                x.DispatchAsync(It.IsAny<Command>()), Times.Once());
        }

        [Test]
        public void Invalid_command_is_not_processed_and_dispatched()
        {
            commandValidatorMock
                .Setup(x => x.IsCommand(It.IsAny<GatewayMessage>()))
                .Returns(false);

            commandProcessor.ProcessMessageAsync(testPayload)
                .Wait();

            commandDispatcherMock.Verify(x =>
                x.DispatchAsync(It.IsAny<Command>()), Times.Never());
        }
    }
}
