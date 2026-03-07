using Microsoft.EntityFrameworkCore;
using smartFan.Data;
using smartFan.Models;
using smartFan.Repositories.Interfaces;

namespace smartFan.Repositories.EfCore
{
    public class DeviceConfigRepository : IDeviceConfigRepository
    {
        private readonly smartFanContext _context;

        public DeviceConfigRepository(smartFanContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DeviceConfig>> GetAllAsync()
        {
            return await _context.DeviceConfigs
                .Include(dc => dc.TemperatureLogs)
                .Include(dc => dc.ManualOverride)
                .Include(dc => dc.ErrorLogs)
                .ToListAsync();
        }

        public async Task<DeviceConfig?> GetByIdAsync(int id)
        {
            return await _context.DeviceConfigs
                .Include(dc => dc.TemperatureLogs)
                .Include(dc => dc.ManualOverride)
                .Include(dc => dc.ErrorLogs)
                .FirstOrDefaultAsync(dc => dc.Id == id);
        }

        public async Task AddAsync(DeviceConfig entity)
        {
            await _context.DeviceConfigs.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DeviceConfig entity)
        {
            _context.DeviceConfigs.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.DeviceConfigs.FindAsync(id);
            if (entity != null)
            {
                _context.DeviceConfigs.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}