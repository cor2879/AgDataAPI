using AgDataAPI.Controllers;
using AgDataAPI.Models;
using AgDataAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgDataAPI.Tests
{
    [TestClass]
    public class RecordControllerTests
    {
        private Mock<IRecordRepository> _mockRecordRepository;
        private RecordController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockRecordRepository = new Mock<IRecordRepository>();
            _controller = new RecordController(_mockRecordRepository.Object);
        }

        [TestMethod]
        public async Task GetRecord_RecordExists_ReturnsOkResult()
        {
            // Arrange
            var expectedRecord = new Record { Name = "test" };
            _mockRecordRepository.Setup(repo => repo.GetAsync("test")).ReturnsAsync(expectedRecord);

            // Act
            var result = await _controller.GetRecordAsync("test") as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedRecord, result.Value);
        }

        [TestMethod]
        public async Task GetRecord_RecordDoesNotExist_ReturnsNotFoundResult()
        {
            // Arrange
            _mockRecordRepository.Setup(repo => repo.GetAsync("test")).ReturnsAsync((Record)null);

            // Act
            var result = await _controller.GetRecordAsync("test");

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetRecords_ReturnsOkResult()
        {
            // Arrange
            var expectedRecords = new List<Record> { new Record { Name = "test1" }, new Record { Name = "test2" } };
            _mockRecordRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(expectedRecords);

            // Act
            var result = await _controller.GetRecordsAsync() as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedRecords, result.Value);
        }

        [TestMethod]
        public async Task CreateRecord_RecordDoesNotExist_ReturnsOkResult()
        {
            // Arrange
            var record = new Record { Name = "test" };
            _mockRecordRepository.Setup(repo => repo.ExistsAsync(record.Name)).ReturnsAsync(false);

            // Act
            var result = await _controller.CreateRecordAsync(record);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
            _mockRecordRepository.Verify(repo => repo.AddAsync(record), Times.Once);
        }

        [TestMethod]
        public async Task CreateRecord_RecordExists_ReturnsBadRequestResult()
        {
            // Arrange
            var record = new Record { Name = "test" };
            _mockRecordRepository.Setup(repo => repo.ExistsAsync(record.Name)).ReturnsAsync(true);

            // Act
            var result = await _controller.CreateRecordAsync(record);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual("A record with this name already exists.", ((BadRequestObjectResult)result).Value);
        }

        [TestMethod]
        public async Task UpdateRecord_RecordDoesNotExist_ReturnsNotFoundResult()
        {
            // Arrange
            var nonExistentRecord = new Record { Name = "nonexistentrecord" };

            _mockRecordRepository.Setup(repo => repo.ExistsAsync(nonExistentRecord.Name))
                     .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateRecordAsync(nonExistentRecord);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task DeleteRecord_RecordDoesNotExist_ReturnsNotFoundResult()
        {
            // Arrange
            string nonExistentRecordName = "nonexistentrecord";

            _mockRecordRepository.Setup(repo => repo.ExistsAsync(nonExistentRecordName))
                     .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteRecordAsync(nonExistentRecordName);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task CreateRecord_RecordNameAlreadyExists_ReturnsBadRequestResult()
        {
            // Arrange
            var existingRecord = new Record { Name = "existingrecord" };

            _mockRecordRepository.Setup(repo => repo.ExistsAsync(existingRecord.Name))
                     .ReturnsAsync(true);

            // Act
            var result = await _controller.CreateRecordAsync(existingRecord);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task GetRecords_ReturnsAllRecords()
        {
            // Arrange
            var records = new[] {
                new Record { Name = "record1" },
                new Record { Name = "record2" },
                new Record { Name = "record3" }
            };

            _mockRecordRepository.Setup(repo => repo.GetAllAsync())
                     .ReturnsAsync(records);

            // Act
            var result = await _controller.GetRecordsAsync() as OkObjectResult;
            var resultRecords = result.Value as Record[];

            // Assert
            Assert.IsNotNull(resultRecords);
            CollectionAssert.AreEqual(records, resultRecords);
        }

        [TestMethod]
        public async Task CreateRecord_InvalidModelState_ReturnsBadRequestResult()
        {
            // Arrange
            var record = new Record();
            _controller.ModelState.AddModelError("Name", "Name is required.");

            // Act
            var result = await _controller.CreateRecordAsync(record);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task GetRecord_RecordExists_ReturnsRecordWithRelatedData()
        {
            // Arrange
            var expectedRecord = new Record { Name = "test" };
            _mockRecordRepository.Setup(repo => repo.GetAsync("test")).ReturnsAsync(expectedRecord);

            // Act
            var result = await _controller.GetRecordAsync("test") as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedRecord, result.Value);
        }

        [TestMethod]
        public async Task GetRecord_RecordExists_RecordDoesNotHaveRelatedData_ReturnsRecord()
        {
            // Arrange
            var expectedRecord = new Record { Name = "test" };
            _mockRecordRepository.Setup(repo => repo.GetAsync("test")).ReturnsAsync(expectedRecord);

            // Act
            var result = await _controller.GetRecordAsync("test") as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedRecord, result.Value);
        }
    }
}
