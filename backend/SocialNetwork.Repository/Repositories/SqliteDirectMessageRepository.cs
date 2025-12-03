using Microsoft.Data.Sqlite;
using SocialNetwork.Entity;
using SocialNetwork.Repository.Interfaces;

namespace SocialNetwork.Repository.Repositories
{
    public class SqliteDirectMessageRepository : IDirectMessageRepository
    {
        private readonly string _connectionString;

        public SqliteDirectMessageRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task AddAsync(DirectMessage directMessage)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO direct_messages (id, senderID, receiverID, message, createdAt)
                VALUES ($id, $senderID, $receiverID, $message, $createdAt);";

            command.Parameters.AddWithValue("$id", directMessage.Id.ToString());
            command.Parameters.AddWithValue("$senderID", directMessage.SenderId.ToString());
            command.Parameters.AddWithValue("$receiverID", directMessage.ReceiverId.ToString());
            command.Parameters.AddWithValue("$message", directMessage.Message);
            command.Parameters.AddWithValue("$createdAt", directMessage.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));

            await command.ExecuteNonQueryAsync();
        }
    }
}

