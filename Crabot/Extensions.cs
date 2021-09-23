using System;
using System.Reflection;
using Autofac;
using Crabot.Commands;
using Crabot.Commands.Commands;
using Crabot.Commands.Dispatcher;
using Crabot.Commands.Handlers.Games.Blackjack;
using Crabot.Core.Repositories;
using Crabot.Gateway;
using Crabot.Rest.RestClient;
using Crabot.WebSocket;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Crabot
{
    internal static class Extensions
    {
        internal static IServiceCollection AddDicordRestClient(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHttpClient<IDiscordRestClient, DiscordRestClient>(config =>
            {
                config.BaseAddress = new Uri(string.Format(configuration["discordApiUrl"],
                    configuration["discordApiVersion"]));
                config.DefaultRequestHeaders.Add("Authorization", string.Format("Bot {0}",
                    Environment.GetEnvironmentVariable("BOT_TOKEN")));
            });

            return services;
        }

        public static ContainerBuilder RegisterGatewayEventHandlers(this ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<EventGatewayDispatcher>()
                .As<IGatewayDispatcher>()
                .SingleInstance();

            containerBuilder.RegisterAssemblyTypes(typeof(IGatewayEventHandler<>).Assembly)
                .AsClosedTypesOf(typeof(IGatewayEventHandler<>))
                .InstancePerLifetimeScope();

            return containerBuilder;
        }

        public static ContainerBuilder RegisterSqliteConnections(this ContainerBuilder containerBuilder)
        {
            containerBuilder.Register(ctx =>
            {
                var address = ctx.Resolve<IConfiguration>()
                    .GetValue<string>("trackedMessagesDatabase");

                return new TrackedMessageRepository(new SqliteConnection(address));
            })
            .As<ITrackedMessageRepository>()
            .InstancePerLifetimeScope();

            containerBuilder.Register(ctx =>
            {
                var address = ctx.Resolve<IConfiguration>()
                    .GetValue<string>("usersDatabase");

                return new SqliteUserPointsRepository(new SqliteConnection(address));
            })
            .As<IUserPointsRepository>()
            .InstancePerLifetimeScope();

            return containerBuilder;
        }

        public static ContainerBuilder RegisterGatewayConnections(this ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<EventGatewayConnection>()
                .As<IGatewayConnection>()
                .Named<IGatewayConnection>("event-gateway-connection")
                .SingleInstance();

            containerBuilder.RegisterType<VoiceGatewayConnection>()
                .As<IGatewayConnection>()
                .Named<IGatewayConnection>("voice-gateway-connection")
                .SingleInstance();

            return containerBuilder;
        }

        public static ContainerBuilder RegisterRepositories(this ContainerBuilder containerBuilder)
        {
            var commandAssembly = typeof(UserPointsRepository).Assembly;

            containerBuilder.RegisterAssemblyTypes(commandAssembly)
                .Where(x => x.Name.EndsWith("Repository"))
                .Except<SqliteUserPointsRepository>()
                .Except<TrackedMessageRepository>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            return containerBuilder;
        }

        public static ContainerBuilder RegisterCommandHandlers(this ContainerBuilder containerBuilder)
        {
            var commandAssembly = typeof(CommandProcessor).Assembly;

            // Register ICommandHandler as Handler with command name as Key for resolving
            containerBuilder.RegisterAssemblyTypes(commandAssembly)
                .Where(x => x.Name.EndsWith("CommandHandler"))
                .Keyed(t => t.GetCustomAttribute<CommandAttribute>().CommandName, typeof(ICommandHandler))
                .As<ICommandHandler>()
                .InstancePerLifetimeScope();

            return containerBuilder;
        }

        public static ContainerBuilder RegisterGameMenager(this ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<BlackjackRepository>()
                .As<IBlackjackRepository>()
                .SingleInstance();

            return containerBuilder;
        }

        public static ContainerBuilder RegisterReactionHandlers(this ContainerBuilder containerBuilder)
        {
            var commandAssembly = typeof(CommandProcessor).Assembly;

            // Register ICommandHandler as Handler with command name as Key for resolving
            containerBuilder.RegisterAssemblyTypes(commandAssembly)
                .Where(x => x.Name.EndsWith("CommandHandler"))
                .Keyed(t => t.GetCustomAttribute<CommandAttribute>().CommandName, typeof(IReactionHandler))
                .As<IReactionHandler>()
                .InstancePerLifetimeScope();

            return containerBuilder;
        }
    }
}
