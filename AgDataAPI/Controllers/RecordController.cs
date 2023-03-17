using Microsoft.AspNetCore.Mvc;

namespace AgDataAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecordController : ControllerBase
    {
        private readonly IRecordRepository _recordRepository;

        public RecordController(IRecordRepository recordRepository)
        {
            _recordRepository = recordRepository;
        }

        // POST api/record
        [HttpPost]
        public async Task<IActionResult> CreateRecordAsync([FromBody] Record record)
        {
            if (string.IsNullOrWhiteSpace(record.Name))
            {
                return BadRequest("A null or blank name is invalid.");
            }

            // Check if the record name already exists
            if (await _recordRepository.ExistsAsync(record.Name))
            {
                return BadRequest("A record with this name already exists.");
            }

            // Add the new record to the database
            await _recordRepository.AddAsync(record);

            return Ok();
        }

        // GET api/record
        [HttpGet]
        public async Task<IActionResult> GetRecordsAsync()
        {
            var records = await _recordRepository.GetAllAsync();

            return Ok(records);
        }

        // GET api/record/{name}
        [HttpGet("{name}")]
        public async Task<IActionResult> GetRecordAsync(string name)
        {
            var record = await _recordRepository.GetAsync(name);

            if (record == null)
            {
                return NotFound();
            }

            return Ok(record);
        }

        // PUT api/record/{name}
        [HttpPut("{name}")]
        public async Task<IActionResult> UpdateRecordAsync([FromBody] Record record)
        {
            // Check if the record to update exists
            if (!await _recordRepository.ExistsAsync(record.Name))
            {
                return NotFound();
            }

            // Update the record in the database
            return await _recordRepository.UpdateAsync(record) ? Ok() : BadRequest();
        }

        // DELETE api/record/{name}
        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteRecordAsync(string name)
        {
            // Check if the record to delete exists
            if (!await _recordRepository.ExistsAsync(name))
            {
                return NotFound();
            }

            // Delete the record from the database
           return await _recordRepository.DeleteAsync(name) ? Ok() : BadRequest();
        }
    }
}
