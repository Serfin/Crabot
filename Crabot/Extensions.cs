using System;
using Crabot.Commands.Commands.Handlers;
using Crabot.Commands.Commands.Models;
using Crabot.Commands.Dispatcher;
using Crabot.Commands.Handlers;
using Crabot.Commands.Models;
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

        internal static IServiceCollection AddCommands(this IServiceCollection services)
        {
            services.AddTransient<ICommandHandler<PingCommand>, PingCommandHandler>();
            services.AddTransient<ICommandHandler<CommandError>, CommandErrorHandler>();
            services.AddTransient<ICommandHandler<HelpCommand>, HelpCommandHandler>();
            services.AddTransient<ICommandHandler<MonsterCommand>, MonsterCommandHandler>();

            return services;
        }
    }
}
