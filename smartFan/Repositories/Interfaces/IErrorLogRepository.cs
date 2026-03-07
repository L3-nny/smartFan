using smartFan.Models;

namespace smartFan.Repositories.Interfaces
{
    public interface IErrorLogRepository : IRepository<ErrorLog>
    {
        Task<IEnumerable<ErrorLog>> GetByDeviceIdAsync(int deviceId);
    }
}