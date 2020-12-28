using System;
using Crabot.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace Crabot.Core.Repositories
{
    public class ClientInfoRepository : IClientInfoRepository
    {
        private const string ClientInfoCacheKey = "Crabot_ClientInfo";

        private readonly IMemoryCache _memoryCache;

        public ClientInfoRepository(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void AddClientInfo(ClientInfo clientInfo)
        {
            if (GetClientInfo() != null)
            {
                throw new ApplicationException("Client info is already added");
            }

            _memoryCache.Set(ClientInfoCacheKey, clientInfo);

        }

        public ClientInfo GetClientInfo()
        {
            return _memoryCache.Get<ClientInfo>(ClientInfoCacheKey);
        }

        public void DeleteClientInfo()
        {
            var clientInfo = _memoryCache.Get<ClientInfo>(ClientInfoCacheKey);

            if (clientInfo != null)
            {
                _memoryCache.Remove(ClientInfoCacheKey);
            }
        }
    }
}
