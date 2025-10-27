using Microsoft.EntityFrameworkCore;
using smartFan.Data;
using smartFan.Models;
using smartFan.Repositories.Interfaces;

namespace smartFan.Repositories.EfCore
{
    public class ErrorLogRepository : IErrorLogRepository
    {
        private readonly smartFanContext _context;

        public ErrorLogRepository(smartFanContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ErrorLog>> GetAllAsync()
        {
            return await _context.ErrorLogs
                .Include(e => e.DeviceConfig)
                .ToListAsync();
        }

        public async Task<ErrorLog?> GetByIdAsync(int id)
        {
            return await _context.ErrorLogs
                .Include(e => e.DeviceConfig)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<ErrorLog>> GetByDeviceIdAsync(int deviceId)
        {
            return await _context.ErrorLogs
                .Include(e => e.DeviceConfig)
                .Where(e => e.DeviceConfigId == deviceId)
                .ToListAsync();
        }

        public async Task AddAsync(ErrorLog entity)
        {
            _context.ErrorLogs.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ErrorLog entity)
        {
            _context.ErrorLogs.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var errorLog = await _context.ErrorLogs.FindAsync(id);
            if (errorLog != null)
            {
                _context.ErrorLogs.Remove(errorLog);
                await _context.SaveChangesAsync();
            }
        }
    }
}
