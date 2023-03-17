using AgDataAPI.Models;

namespace AgDataAPI.Interfaces; 

public interface IRecordRepository
{
    Task AddAsync(Record record);

    Task<bool> UpdateAsync(Record record);

    Task<bool> DeleteAsync(string name);

    Task<Record> GetAsync(string name);

    Task<Record> GetAsync(int id);

    Task<ICollection<Record>> GetAllAsync();

    Task<bool> ExistsAsync(string name);
}
