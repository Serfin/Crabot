using Crabot.Contracts;

namespace Crabot.Repositories
{
    public interface IClientInfoRepository
    {
        void AddClientInfo(ClientInfo clientInfo);
        ClientInfo GetClientInfo();
    }
}
