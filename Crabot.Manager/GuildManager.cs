using System;
using System.Collections.Generic;
using System.Text;

namespace Crabot.Manager
{
    public class GuildManager
    {
        public void RegisterGuild(Guild guild)
        {
            if (_guilds.Any(x => x.Id == guild.Id))
            {
                throw new ApplicationException("Guid is already registered!");
            }

            _guilds.Add(guild);
        }

        public void DeregisterGuild(Guild guild)
        {
            if (_guilds.Any(x => x.Id == guild.Id))
            {
                _guilds.Remove(guild);
                return;
            }

            throw new ApplicationException("Guid is not registered!");
        }
    }
}
