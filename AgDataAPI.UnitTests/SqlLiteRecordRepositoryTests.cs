#pragma warning disable CS8618
using System.Data.SQLite;
using System.Threading.Tasks;

namespace AgDataAPI.UnitTests
{
    [TestClass]
    public class SQLiteRecordRepositoryTests
    {
        private SQLiteRecordRepository _repository;
        private const string ConnectionString = "Data Source=./Data/AgDataAPI.db;Mode=ReadWriteCreate;";

        [TestInitialize]
        public void Setup()
        {
            _repository = new SQLiteRecordRepository(ConnectionString);
        }

        private async Task ClearDataAsync()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                await connection.OpenAsync();

                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"delete from Records";

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        private async Task SeedDataAsync()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                await connection.OpenAsync();

                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Records (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Address TEXT NOT NULL
                );
            ";
                    await command.ExecuteNonQueryAsync();

                    command.CommandText = @"
                INSERT INTO Records (ID, Name, Address) VALUES
                (1, 'John Smith', '123 Main St'),
                (2, 'Jane Doe', '456 Oak Ave'),
                (3, 'Bob Johnson', '789 Maple Rd');
            ";
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        [TestMethod]
        public async Task GetAll_ReturnsAllRecordsAsync()
        {
            await ClearDataAsync();

            // Arrange
            var expectedRecords = new List<Record>
            {
                new Record { Id = 1, Name = "John Doe", Address = "123 Main St" },
                new Record { Id = 2, Name = "Jane Smith", Address = "456 High St" },
                new Record { Id = 3, Name = "Bob Johnson", Address = "789 Maple Ave" }
            };

            // Act
            var actualRecords = (await _repository.GetAllAsync()).ToList();

            // Assert
            for (var i = 0; i < actualRecords.Count; i++)
            {
                Assert.AreEqual(expectedRecords[i].Id, actualRecords[i].Id);
                Assert.AreEqual(expectedRecords[i].Name, actualRecords[i].Name);
                Assert.AreEqual(expectedRecords[i].Address, actualRecords[i].Address);
            }
        }

        [TestMethod]
        public async Task Add_RecordDoesNotExist_RecordAddedAsync()
        {
            await ClearDataAsync();

            // Arrange
            var newRecord = new Record { Name = "New Person", Address = "999 New St" };

            // Act
            await _repository.AddAsync(newRecord);
            var actualRecord = await _repository.GetAsync(newRecord.Name);

            // Assert
            Assert.AreEqual(newRecord.Name, actualRecord.Name);
            Assert.AreEqual(newRecord.Address, actualRecord.Address);
        }

        [TestMethod]
        public async Task Add_RecordExists_ThrowsArgumentExceptionAsync()
        {
            await ClearDataAsync();
            await SeedDataAsync();

            // Arrange
            var existingRecord = new Record { Id = 1, Name = "Jane Smith", Address = "999 New St" };

            try
            {
                // Act
                await _repository.AddAsync(existingRecord);

                Assert.IsTrue(false, "Exception was not thrown");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is ArgumentException);
            }
        }

        [TestMethod]
        public async Task Update_RecordExists_RecordUpdatedAsync()
        {
            await ClearDataAsync();
            await SeedDataAsync();

            // Arrange
            var updatedRecord = new Record { Id = 2, Name = "Updated Person", Address = "888 Updated St" };

            // Act
            await _repository.UpdateAsync(updatedRecord);
            var actualRecord = await _repository.GetAsync(2);

            // Assert
            Assert.AreEqual(updatedRecord.Id, actualRecord.Id);
            Assert.AreEqual(updatedRecord.Name, actualRecord.Name);
            Assert.AreEqual(updatedRecord.Address, actualRecord.Address);
        }

        [TestMethod]
        public async Task Update_RecordDoesNotExist_ThrowsArgumentExceptionAsync()
        {
            await ClearDataAsync();
            await SeedDataAsync();

            try
            {
                // Arrange
                var nonExistentRecord = new Record { Id = 999, Name = "Does Not Exist", Address = "123 Fake St" };

                // Act
                await _repository.UpdateAsync(nonExistentRecord);

                Assert.IsTrue(false, "Exception was not thrown");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is ArgumentException);
            }
        }
    }
}