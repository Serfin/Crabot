using Autofac;
using Autofac.Extras.Moq;
using Crabot.Commands.Dispatcher;
using Crabot.Contracts;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Crabot.Tests
{
    [TestFixture]
    public class CommandDispatcherTests
    {
        private Mock<ILogger<CommandDispatcher>> loggerMock;
        private IComponentContext componentContext;

        private CommandDispatcher commandDispatcher;

        private Command nonExistentCommand = new Command(
            new GatewayMessage
            {
                Content = "?test-command-that-is-not-registered"
            });

        [SetUp]
        public void Setup()
        {
            loggerMock = new Mock<ILogger<CommandDispatcher>>();

            using (var mock = AutoMock.GetLoose())
            {
                componentContext = mock.Create<IComponentContext>();

                commandDispatcher = new CommandDispatcher(
                    componentContext, loggerMock.Object);
            }
        }

        [Test]
        public void Not_registered_command_should_be_processed_as_error_command()
        {
            

            commandDispatcher.DispatchAsync(nonExistentCommand).Wait();
        }
    }
}
