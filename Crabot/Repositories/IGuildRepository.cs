using Crabot.Contracts;

namespace Crabot.Repositories
{
    public interface IGuildRepository
    {
        void AddGuild(Guild guild);
        Guild GetGuild(string id);
        void DeleteGuild(string id);
    }
}
