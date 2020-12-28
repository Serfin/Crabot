using Crabot.Contracts;

namespace Crabot.Core.Repositories
{
    public interface IClientInfoRepository
    {
        void AddClientInfo(ClientInfo clientInfo);
        ClientInfo GetClientInfo();
        void DeleteClientInfo();
    }
}
