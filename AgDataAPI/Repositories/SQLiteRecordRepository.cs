#pragma warning disable CS8603
using System.Data.SQLite;

namespace AgDataAPI.Repositories;

public class SQLiteRecordRepository : IRecordRepository
{
    private string _connectionString;

    public SQLiteRecordRepository(string connectionString)
    {
        this._connectionString = connectionString;

        this.InitializeDatabase().Wait();
    }

    private async Task InitializeDatabase()
    {
        using var connection = new SQLiteConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SQLiteCommand(connection);
        command.CommandText = @"
    CREATE TABLE IF NOT EXISTS Records (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        Name TEXT NOT NULL,
        Address TEXT NOT NULL
    );

    CREATE UNIQUE INDEX IF NOT EXISTS idx_Records_Name ON Records (Name);
";
        await command.ExecuteNonQueryAsync();
    }


    public async Task AddAsync(Record record)
    {
        using (var connection = new SQLiteConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new SQLiteCommand("INSERT INTO Records (Id, Name, Address) VALUES (@id, @name, @address)", connection);
            command.Parameters.AddWithValue("id", record.Id);
            command.Parameters.AddWithValue("@name", record.Name);
            command.Parameters.AddWithValue("@address", record.Address);

            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error inserting record", "record", ex);
            }
        }
    }

    public async Task<bool> UpdateAsync(Record record)
    {
        if (await GetAsync(record.Id) == null)
        {
            throw new ArgumentException("Record does not exist", "record");
        }

        using (var connection = new SQLiteConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new SQLiteCommand("UPDATE Records SET Name = @name, Address = @address WHERE ID = @id", connection);
            command.Parameters.AddWithValue("id", record.Id);
            command.Parameters.AddWithValue("@name", record.Name);
            command.Parameters.AddWithValue("@address", record.Address);

            return await command.ExecuteNonQueryAsync() > 0;
        }
    }

    public async Task<bool> DeleteAsync(string name)
    {
        using (var connection = new SQLiteConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new SQLiteCommand("DELETE FROM Records WHERE Name = @name", connection);
            command.Parameters.AddWithValue("@name", name);

            return await command.ExecuteNonQueryAsync() > 0;
        }
    }

    public async Task<Record> GetAsync(string name)
    {
        using (var connection = new SQLiteConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new SQLiteCommand("SELECT * FROM Records WHERE Name = @name", connection);
            command.Parameters.AddWithValue("@name", name);

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new Record
                    {
                        Id = (int)(long)reader["Id"],
                        Name = (string)reader["Name"],
                        Address = (string)reader["Address"]
                    };
                }
                else
                {
                    return null;
                }
            }
        }
    }

    public async Task<Record> GetAsync(int id)
    {
        using (var connection = new SQLiteConnection(this._connectionString))
        {
            await connection.OpenAsync();

            var command = new SQLiteCommand("SELECT * FROM Records WHERE Id = @id", connection);
            command.Parameters.AddWithValue("@id", id);

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new Record
                    {
                        Id = (int)(long)reader["Id"],
                        Name = (string)reader["Name"],
                        Address = (string)reader["Address"]
                    };
                }
                else
                {
                    return null;
                }
            }
        }
    }

    public async Task<ICollection<Record>> GetAllAsync()
    {
        var records = new List<Record>();

        using (var connection = new SQLiteConnection(this._connectionString))
        {
            await connection.OpenAsync();

            var command = new SQLiteCommand("SELECT * FROM Records", connection);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    records.Add(new Record
                    {
                        Id = (int)(long)reader["Id"],
                        Name = (string)reader["Name"],
                        Address = (string)reader["Address"]
                    });
                }
            }
        }

        return records;
    }

    public async Task<bool> ExistsAsync(string name)
    {
        using (var connection = new SQLiteConnection(this._connectionString))
        {
            await connection.OpenAsync();

            var command = new SQLiteCommand("SELECT COUNT(*) FROM Records WHERE Name = @name", connection);
            command.Parameters.AddWithValue("@name", name);

            return ((long)await command.ExecuteScalarAsync()) > 0;
        }
    }
}
