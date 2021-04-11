using System.IO;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Crabot.Commands;
using Crabot.Commands.Commands;
using Crabot.Commands.Dispatcher;
using Crabot.Gateway;
using Crabot.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using Serilog.Events;

namespace Crabot
{
    internal class Program
    {
        private static IConfiguration _configuration;

        internal static async Task Main(string[] args)
        {
            ConfigureSerialization();

            using (var services = ConfigureServices())
            {
                await services.GetService<CrabotClient>().StartAsync();
                await Task.Delay(-1);
            }
        }

        private static AutofacServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddSingleton(LoadConfiguration())
                .AddLogging(builder =>
                {
                    builder.ClearProviders();
                    builder.AddSerilog(LoadLoggerConfiguration());
                })
                .AddSingleton<CrabotClient>()
                .AddSingleton<DiscordGatewayClient>()
                .AddMemoryCache()
                .AddTransient<ICommandValidator, CommandValidator>()
                .AddTransient<ICommandProcessor, CommandProcessor>()
                .AddTransient<IGatewayEventDispatcher, GatewayEventDispatcher>()
                .AddTransient<ICommandDispatcher, CommandDispatcher>()
                .AddSingleton<IConnectionManager, ConnectionManager>()
                .AddDicordRestClient(_configuration);

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            containerBuilder.RegisterCommandHandlers();
            containerBuilder.RegisterSqliteConnections();
            containerBuilder.RegisterGatewayEventHandlers();
            containerBuilder.RegisterDiscordSocketClient();
            containerBuilder.RegisterRepositories();

            var container = containerBuilder.Build();

            return new AutofacServiceProvider(container);
        }

        private static Serilog.ILogger LoadLoggerConfiguration()
        {
            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs\\Crabot.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        private static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();

            return _configuration;
        }

        private static void ConfigureSerialization()
        {
            JsonConvert.DefaultSettings = () =>
            {
                var settings = new JsonSerializerSettings();
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                settings.NullValueHandling = NullValueHandling.Ignore;

                return settings;
            };
        }
    }
}
