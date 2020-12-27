using Crabot.Contracts;

namespace Crabot.Core.Repositories
{
    public interface IGuildRepository
    {
        void AddGuild(Guild guild);
        Guild GetGuild(string id);
        void DeleteGuild(string id);
    }
}
