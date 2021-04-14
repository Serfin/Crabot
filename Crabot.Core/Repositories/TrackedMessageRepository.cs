using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Crabot.Core.Repositories
{
    // create table tracked_messages(guildId varchar(18), channelId varchar(18), messageId varchar(18) primary key, command text, validEmojis text);
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
                        INSERT INTO tracked_messages VALUES (@guildId, @channelId, @messageId, @command, @validEmojis)
                    ";

                command.Parameters.AddWithValue("@guildId", trackedMessage.GuildId);
                command.Parameters.AddWithValue("@channelId", trackedMessage.ChannelId);
                command.Parameters.AddWithValue("@messageId", trackedMessage.MessageId);
                command.Parameters.AddWithValue("@command", trackedMessage.Command);
                command.Parameters.AddWithValue("@validEmojis", ToCommaSeparatedValue(trackedMessage.ValidEmojis));

                await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }
        }

        public async Task<TrackedMessage> GetTrackedMessageAsync(string messageId)
        {
            using (var connection = _sqliteConnection)
            {
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText =
                    @$"
                        SELECT * FROM tracked_messages WHERE messageId = @messageId
                    ";

                command.Parameters.AddWithValue("@messageId", messageId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        return new TrackedMessage(reader.GetString(0), reader.GetString(1),
                            reader.GetString(2), reader.GetString(3), ToDictionary(reader.GetString(4)));
                    }

                    await reader.CloseAsync();
                }

                await connection.CloseAsync();
            }

            return null;
        }

        public async Task RemoveTrackedMessageAsync(string channelId, string messageId)
        {
            using (var connection = _sqliteConnection)
            {
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText =
                    @$"
                        DELETE FROM tracked_messages WHERE channelId = @channelId AND messageId = @messageId
                    ";

                command.Parameters.AddWithValue("@channelId", channelId);
                command.Parameters.AddWithValue("@messageId", messageId);

                await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }
        }

        private string ToCommaSeparatedValue(IReadOnlyDictionary<string, string> source)
        {
            var result = string.Empty;

            foreach (var key in source.Keys)
            {
                result += key + ';' + source.GetValueOrDefault(key) + ';';
            }

            return result;
        }

        private IReadOnlyDictionary<string, string> ToDictionary(string source)
        {
            var split = source.Split(';', StringSplitOptions.RemoveEmptyEntries);

            var result = new Dictionary<string, string>();
            for (var i = 0; i < split.Length; i += 2)
            {
                result.Add(split[i], split[i + 1]);
            }

            return result;
        }
    }
}
