﻿using System.IO;
using System.Threading.Tasks;
using Crabot.Commands;
using Crabot.Commands.Commands;
using Crabot.Commands.Dispatcher;
using Crabot.Core.Repositories;
using Crabot.Gateway;
using Crabot.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Crabot
{
    internal class Program
    {
        private static IConfiguration _configuration;

        internal static async Task Main(string[] args)
        {
            using (var services = ConfigureServices())
            {
                await services.GetService<CrabotClient>().StartAsync();
                await Task.Delay(-1);
            }
        }

        private static ServiceProvider ConfigureServices()
            => new ServiceCollection()
                .AddLogging(config => config.AddConsole())
                .AddSingleton(LoadConfiguration())
                .AddSingleton<CrabotClient>()
                .AddSingleton<DiscordGatewayClient>()
                .AddMemoryCache()
                .AddTransient<ICommandValidator, CommandValidator>()
                .AddTransient<ICommandProcessor, CommandProcessor>()
                .AddTransient<IGatewayEventDispatcher, GatewayEventDispatcher>()
                .AddTransient<ICommandDispatcher, CommandDispatcher>()
                .AddGatewayEventHandlers()
                .AddCommandHandlers()
                .AddSingleton<IConnectionManager, ConnectionManager>()
                .AddTransient<IGuildRepository, GuildRepository>()
                .AddTransient<IClientInfoRepository, ClientInfoRepository>()
                .AddDicordRestClient(_configuration)
                .AddDiscordSocketClient()
                .BuildServiceProvider();

        private static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();

            return _configuration;
        }
    }
}
