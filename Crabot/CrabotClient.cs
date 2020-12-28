using System.Threading.Tasks;
using Crabot.Gateway;

namespace Crabot
{
    public class CrabotClient
    {
        private readonly DiscordGatewayClient _discordGatewayClient;

        public CrabotClient(DiscordGatewayClient discordGatewayClient)
        {
            _discordGatewayClient = discordGatewayClient;
        }

        public async Task StartAsync()
        {
            await _discordGatewayClient.StartAsync();
        }
    }
}
