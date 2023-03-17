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

            if (await _recordRepository.ExistsAsync(record.Name))
            {
                return BadRequest("A record with this name already exists.");
            }

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
            if (!await _recordRepository.ExistsAsync(record.Name))
            {
                return NotFound();
            }

            return await _recordRepository.UpdateAsync(record) ? Ok() : BadRequest();
        }

        // DELETE api/record/{name}
        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteRecordAsync(string name)
        {
            if (!await _recordRepository.ExistsAsync(name))
            {
                return NotFound();
            }

           return await _recordRepository.DeleteAsync(name) ? Ok() : BadRequest();
        }
    }
}
