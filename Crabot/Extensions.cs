using System;
using Crabot.Commands.Dispatcher;
using Crabot.Commands.Handlers;
using Crabot.Commands.Models;
using Crabot.Contracts;
using Crabot.EventHandlers;
using Crabot.Gateway.SocketClient;
using Crabot.Models;
using Crabot.Rest.RestClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Crabot
{
    internal static class Extensions
    {
        internal static void AddDicordRestClient(this IServiceCollection services, 
            IConfiguration configuration)
        {
            services.AddHttpClient<IDiscordRestClient, DiscordRestClient>(config =>
            {
                config.BaseAddress = new Uri(string.Format(configuration["discordApiUrl"], 
                    configuration["discordApiVersion"]));
                config.DefaultRequestHeaders.Add("Authorization", string.Format("Bot {0}",
                    Environment.GetEnvironmentVariable("BOT_TOKEN")));
            });
        }

        internal static void AddDiscordSocketClient(this IServiceCollection services)
        {
            services.AddSingleton<IDiscordSocketClient, DiscordSocketClient>();
        }

        internal static void AddGatewayEventHandlers(this IServiceCollection services)
        {
            services.AddTransient<IGatewayEventHandler<IdentifyEvent>, IdentifyEventHandler>();
            services.AddTransient<IGatewayEventHandler<HeartbeatEvent>, HeartbeatEventHandler>();
            services.AddTransient<IGatewayEventHandler<MessageCreatedEvent>, MessageCreatedEventHandler>();
            services.AddTransient<IGatewayEventHandler<ReadyEvent>, ReadyEventHandler>();
            services.AddTransient<IGatewayEventHandler<Guild>, GuildCreateEventHandler>();
            services.AddTransient<IGatewayEventHandler<ResumeEvent>, ResumeEventHandler>();
        }

        internal static void AddCommandHandlers(this IServiceCollection services)
        {
            services.AddTransient<ICommandHandler<PingCommand>, PingCommandHandler>();
            services.AddTransient<ICommandHandler<CommandError>, CommandErrorHandler>();
        }
    }
}
