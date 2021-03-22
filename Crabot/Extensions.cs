using System;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Crabot.Commands;
using Crabot.Commands.Commands;
using Crabot.Commands.Dispatcher;
using Crabot.Contracts;
using Crabot.Core.Events;
using Crabot.Gateway;
using Crabot.Gateway.EventHandlers;
using Crabot.Rest.RestClient;
using Crabot.WebSocket;
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

        internal static IServiceCollection AddDiscordSocketClient(this IServiceCollection services)
        {
            services.AddSingleton<IDiscordSocketClient, DiscordSocketClient>();

            return services;
        }

        internal static IServiceCollection AddGatewayEventHandlers(this IServiceCollection services)
        {
            services.AddTransient<IGatewayEventHandler<ReadyEvent>, ReadyEventHandler>();
            services.AddTransient<IGatewayEventHandler<Guild>, GuildCreateEventHandler>();

            return services;
        }

        public static ContainerBuilder RegisterCommandHandler(this ContainerBuilder containerBuilder)
        {
            var commandAssembly = typeof(CommandProcessor).Assembly;

            // Register ICommandHandler as Handler with command name as Key for resolving
            containerBuilder.RegisterAssemblyTypes(commandAssembly)
                .Where(x => x.Name.EndsWith("CommandHandler"))
                .Keyed(t => t.GetCustomAttribute<CommandAttribute>().CommandName, typeof(ICommandHandler))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            return containerBuilder;
        }
    }
}
