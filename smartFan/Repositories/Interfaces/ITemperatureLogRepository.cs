using smartFan.Models;

namespace smartFan.Repositories.Interfaces
{
    public interface ITemperatureLogRepository : IRepository<TemperatureLog>
    {
        Task<IEnumerable<TemperatureLog>> GetByDeviceIdAsync(int deviceId);
    }
}