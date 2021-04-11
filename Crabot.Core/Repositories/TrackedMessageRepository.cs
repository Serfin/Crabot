using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Crabot.Core.Repositories
{
    // create table tracked_messages(guildId varchar(18) primary key, channelId varchar(18), messageId varchar(18), validEmojis text);
    public class TrackedMessageRepository : ITrackedMessageRepository
    {
        private readonly SqliteConnection _sqliteConnection;

        public TrackedMessageRepository(SqliteConnection sqliteConnection)
        {
            _sqliteConnection = sqliteConnection;
        }

        public async Task AddTrackedMessageAsync(TrackedMessage trackedMessage)
        {
            using (var connection = _sqliteConnection)
            {
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText =
                    @$"
                        INSERT INTO tracked_messages VALUES (@guildId, @channelId, @messageId, @validEmojis)
                    ";

                command.Parameters.AddWithValue("@guildId", trackedMessage.GuildId);
                command.Parameters.AddWithValue("@channelId", trackedMessage.ChannelId);
                command.Parameters.AddWithValue("@messageId", trackedMessage.MessageId);
                command.Parameters.AddWithValue("@validEmojis", ToCommaSeparatedValue(trackedMessage.ValidEmojis));

                await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }
        }

        public async Task RemoveTrackedMessageAsync(TrackedMessage trackedMessage)
        {
            using (var connection = _sqliteConnection)
            {
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText =
                    @$"
                        DELETE FROM tracked_messages WHERE channelId = @channelId AND messageId = @messageId
                    ";

                command.Parameters.AddWithValue("@channelId", trackedMessage.ChannelId);
                command.Parameters.AddWithValue("@messageId", trackedMessage.MessageId);

                await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }
        }

        private string ToCommaSeparatedValue(IReadOnlyDictionary<string, string> source)
        {
            var result = string.Empty;

            foreach (var key in source.Keys)
            {
                result += key + ';' + source.GetValueOrDefault(key);
            }

            return result;
        }
    }
}
