using Microsoft.EntityFrameworkCore;
using smartFan.Data;
using smartFan.Models;
using smartFan.Repositories.Interfaces;

namespace smartFan.Repositories.EfCore
{
    public class TemperatureLogRepository : ITemperatureLogRepository
    {
        private readonly smartFanContext _context;

        public TemperatureLogRepository(smartFanContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TemperatureLog>> GetAllAsync()
        {
            return await _context.TemperatureLogs
                .Include(tl => tl.DeviceConfig)
                .OrderByDescending(tl => tl.Timestamp)
                .ToListAsync();
        }

        public async Task<TemperatureLog?> GetByIdAsync(int id)
        {
            return await _context.TemperatureLogs
                .Include(tl => tl.DeviceConfig)
                .FirstOrDefaultAsync(tl => tl.Id == id);
        }

        public async Task<IEnumerable<TemperatureLog>> GetByDeviceIdAsync(int deviceId)
        {
            return await _context.TemperatureLogs
                .Where(tl => tl.DeviceConfigId == deviceId)
                .OrderByDescending(tl => tl.Timestamp)
                .ToListAsync();
        }

        public async Task AddAsync(TemperatureLog entity)
        {
            await _context.TemperatureLogs.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TemperatureLog entity)
        {
            _context.TemperatureLogs.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.TemperatureLogs.FindAsync(id);
            if (entity != null)
            {
                _context.TemperatureLogs.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}