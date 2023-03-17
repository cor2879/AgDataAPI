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
            var expectedRecord = new Record { Name = "test" };
            _mockRecordRepository.Setup(repo => repo.GetAsync("test")).ReturnsAsync(expectedRecord);

            var result = await _controller.GetRecordAsync("test") as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedRecord, result.Value);
        }

        [TestMethod]
        public async Task GetRecord_RecordDoesNotExist_ReturnsNotFoundResult()
        {
            _mockRecordRepository.Setup(repo => repo.GetAsync("test")).ReturnsAsync((Record)null);

            var result = await _controller.GetRecordAsync("test");

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetRecords_ReturnsOkResult()
        {
            var expectedRecords = new List<Record> { new Record { Name = "test1" }, new Record { Name = "test2" } };
            _mockRecordRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(expectedRecords);

            var result = await _controller.GetRecordsAsync() as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedRecords, result.Value);
        }

        [TestMethod]
        public async Task CreateRecord_RecordDoesNotExist_ReturnsOkResult()
        {
            var record = new Record { Name = "test" };
            _mockRecordRepository.Setup(repo => repo.ExistsAsync(record.Name)).ReturnsAsync(false);

            var result = await _controller.CreateRecordAsync(record);

            Assert.IsInstanceOfType(result, typeof(OkResult));
            _mockRecordRepository.Verify(repo => repo.AddAsync(record), Times.Once);
        }

        [TestMethod]
        public async Task CreateRecord_RecordExists_ReturnsBadRequestResult()
        {
            var record = new Record { Name = "test" };
            _mockRecordRepository.Setup(repo => repo.ExistsAsync(record.Name)).ReturnsAsync(true);

            var result = await _controller.CreateRecordAsync(record);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual("A record with this name already exists.", ((BadRequestObjectResult)result).Value);
        }

        [TestMethod]
        public async Task UpdateRecord_RecordDoesNotExist_ReturnsNotFoundResult()
        {
            var nonExistentRecord = new Record { Name = "nonexistentrecord" };

            _mockRecordRepository.Setup(repo => repo.ExistsAsync(nonExistentRecord.Name))
                     .ReturnsAsync(false);

            var result = await _controller.UpdateRecordAsync(nonExistentRecord);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task DeleteRecord_RecordDoesNotExist_ReturnsNotFoundResult()
        {
            string nonExistentRecordName = "nonexistentrecord";

            _mockRecordRepository.Setup(repo => repo.ExistsAsync(nonExistentRecordName))
                     .ReturnsAsync(false);

            var result = await _controller.DeleteRecordAsync(nonExistentRecordName);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task CreateRecord_RecordNameAlreadyExists_ReturnsBadRequestResult()
        {
            var existingRecord = new Record { Name = "existingrecord" };

            _mockRecordRepository.Setup(repo => repo.ExistsAsync(existingRecord.Name))
                     .ReturnsAsync(true);

            var result = await _controller.CreateRecordAsync(existingRecord);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task GetRecords_ReturnsAllRecords()
        {
            var records = new[] {
                new Record { Name = "record1" },
                new Record { Name = "record2" },
                new Record { Name = "record3" }
            };

            _mockRecordRepository.Setup(repo => repo.GetAllAsync())
                     .ReturnsAsync(records);

            var result = await _controller.GetRecordsAsync() as OkObjectResult;
            var resultRecords = result.Value as Record[];

            Assert.IsNotNull(resultRecords);
            CollectionAssert.AreEqual(records, resultRecords);
        }

        [TestMethod]
        public async Task CreateRecord_InvalidModelState_ReturnsBadRequestResult()
        {
            var record = new Record();
            _controller.ModelState.AddModelError("Name", "Name is required.");

            var result = await _controller.CreateRecordAsync(record);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task GetRecord_RecordExists_ReturnsRecordWithRelatedData()
        {
            var expectedRecord = new Record { Name = "test" };
            _mockRecordRepository.Setup(repo => repo.GetAsync("test")).ReturnsAsync(expectedRecord);

            var result = await _controller.GetRecordAsync("test") as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedRecord, result.Value);
        }

        [TestMethod]
        public async Task GetRecord_RecordExists_RecordDoesNotHaveRelatedData_ReturnsRecord()
        {
            var expectedRecord = new Record { Name = "test" };
            _mockRecordRepository.Setup(repo => repo.GetAsync("test")).ReturnsAsync(expectedRecord);

            var result = await _controller.GetRecordAsync("test") as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedRecord, result.Value);
        }
    }
}
