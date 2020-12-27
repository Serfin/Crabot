using System.IO;
using System.Threading.Tasks;
using Crabot.Commands.Dispatcher;
using Crabot.Commands.Handlers;
using Crabot.Commands.Models;
using Crabot.Repositories;
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
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();

            await serviceProvider.GetService<CrabotClient>().StartAsync();
            await Task.Delay(-1);
        }

        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddLogging(config => config.AddConsole());
            services.AddSingleton(LoadConfiguration());
            services.AddSingleton<CrabotClient>();
            services.AddMemoryCache();

            services.AddTransient<IEventDispatcher, EventDispatcher>();
            services.AddTransient<ICommandDispatcher, CommandDispatcher>();

            services.AddGatewayEventHandlers();
            services.AddCommandHandlers();

            services.AddTransient<IGuildRepository, GuildRepository>();
            services.AddTransient<IClientInfoRepository, ClientInfoRepository>();

            services.AddDicordRestClient(_configuration);
            services.AddDiscordSocketClient();

            return services;
        }

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
