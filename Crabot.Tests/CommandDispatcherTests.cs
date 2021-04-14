using Autofac;
using Autofac.Extras.Moq;
using Crabot.Commands.Dispatcher;
using Crabot.Commands.Handlers;
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
        private Mock<NotFoundCommandHandler> commandNotFoundMock;
        private Mock<InternalErrorCommandHandler> commandProcessingErrorMock;

        private ContainerBuilder containerBuilder;
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
            commandNotFoundMock = new Mock<NotFoundCommandHandler>();
            commandNotFoundMock.Setup(x => x.HandleAsync(nonExistentCommand).Wait());

            commandProcessingErrorMock = new Mock<InternalErrorCommandHandler>();

            using (var mock = AutoMock.GetLoose())
            {
                containerBuilder = new ContainerBuilder();
                containerBuilder.RegisterType<NotFoundCommandHandler>()
                    .Keyed<ICommandHandler>("command-not-found")
                    .As<ICommandHandler>();

                containerBuilder.RegisterType<InternalErrorCommandHandler>()
                    .Keyed<ICommandHandler>("internal-application-error")
                    .As<ICommandHandler>();

                var container = containerBuilder.Build();

                //commandDispatcher = new CommandDispatcher(
                //    container.Resolve<IComponentContext>(), loggerMock.Object);
            }
        }

        [Test]
        public void Not_registered_command_should_be_processed_with_not_found_command_handler()
        {
            commandDispatcher.DispatchAsync(nonExistentCommand).Wait();

            commandNotFoundMock.Verify(x => x.HandleAsync(nonExistentCommand), Times.Once);
        }
    }
}
