using System.Threading.Tasks;
using Crabot.Gateway;

namespace Crabot.Core
{
    public interface IConnectionManager
    {
        public void SetSequenceNumber(int? sequenceNumber);
        public Task CreateConnection(GatewayPayload helloEvent);
    }
}
