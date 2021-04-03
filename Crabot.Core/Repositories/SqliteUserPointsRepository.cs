using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Crabot.Core.Repositories
{
    // create table users(userId varchar(18) primary key, userNickname text, balance real, dailyUsedAt text);
    public class SqliteUserPointsRepository : IUserPointsRepository
    {
        private readonly SqliteConnection _sqliteConnection;

        public SqliteUserPointsRepository(SqliteConnection sqliteConnection)
        {
            _sqliteConnection = sqliteConnection;
        }

        public async Task AddBalanceToUserAccount(string userId, float amountToAdd)
        {
            using (var connection = _sqliteConnection)
            {
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText =
                    @$"
                        UPDATE users SET balance = balance + @balance WHERE userId = @userId
                    ";

                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@balance", amountToAdd);

                await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }
        }

        public async Task AddUserToSystem(string nickname, string userId)
        {
            using (var connection = _sqliteConnection)
            {
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText =
                    @$"
                        INSERT INTO users VALUES (@userId, @userNickname, @defaultBalance, @creationDate)
                    ";

                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@userNickname", nickname);
                command.Parameters.AddWithValue("@defaultBalance", default(float));
                command.Parameters.AddWithValue("@creationDate",
                    DateTime.Now.Subtract(TimeSpan.FromMinutes(10)));

                await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }
        }

        public async Task<bool> CanUseDailyPoints(string userId)
        {
            using (var connection = _sqliteConnection)
            {
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText =
                    @$"
                        SELECT dailyUsedAt FROM users WHERE userId = @userId
                    ";

                command.Parameters.AddWithValue("@userId", userId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        return reader.GetDateTime(0).AddMinutes(10) < DateTime.Now;
                    }

                    await reader.CloseAsync();
                }

                await connection.CloseAsync();
            }

            return false;
        }

        public async Task<float?> GetUserBalanceAsync(string userId)
        {
            using (var connection = _sqliteConnection)
            {
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText =
                    @$"
                        SELECT balance FROM users WHERE userId = @userId
                    ";

                command.Parameters.AddWithValue("@userId", userId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        return reader.GetFloat(0);
                    }

                    await reader.CloseAsync();
                }

                await connection.CloseAsync();
            }

            return null;
        }

        public async Task<List<UserPoint>> GetUsersAsync()
        {
            var users = new List<UserPoint>();
            using (var connection = _sqliteConnection)
            {
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText =
                    @$"
                        SELECT * FROM users ORDER BY balance DESC LIMIT 15
                    ";

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        users.Add(new UserPoint(
                                reader.GetString(1),
                                reader.GetString(0),
                                reader.GetFloat(2),
                                reader.GetDateTime(3)));
                    }

                    await reader.CloseAsync();
                }

                await connection.CloseAsync();
            }

            return users;
        }

        public async Task SubtractBalanceFromUserAccount(string userId, float amountToSubtract)
        {
            using (var connection = _sqliteConnection)
            {
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText =
                    @$"
                        UPDATE users SET balance = balance - @balance WHERE userId = @userId
                    ";

                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@balance", amountToSubtract);

                await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }
        }

        public async Task UpdateDailyPoints(string userId)
        {
            using (var connection = _sqliteConnection)
            {
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText =
                    @$"
                        UPDATE users SET dailyUsedAt = @dailyUsedAt WHERE userId = @userId
                    ";

                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@dailyUsedAt", DateTime.Now);

                await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }
        }
    }
}
