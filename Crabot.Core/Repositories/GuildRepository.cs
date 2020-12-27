using System;
using Crabot.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace Crabot.Core.Repositories
{
    public class GuildRepository : IGuildRepository
    {
        private const string GuildCacheKey = "Crabot_Guild_{0}";

        private readonly IMemoryCache _memoryCache;

        public GuildRepository(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void AddGuild(Guild guild)
        {
            if (GetGuild(guild.Id) != null)
            {
                throw new ApplicationException("Guild is already registered!");
            }

            _memoryCache.Set(string.Format(GuildCacheKey, guild.Id), guild);
        }

        public void DeleteGuild(string id)
        {
            _memoryCache.Remove(string.Format(GuildCacheKey, id));
        }

        public Guild GetGuild(string id)
        {
            return _memoryCache.Get<Guild>(string.Format(GuildCacheKey, id));
        }
    }
}
