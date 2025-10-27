using Microsoft.EntityFrameworkCore;
using smartFan.Models;
using System.Collections.Generic;

namespace smartFan.Data
{
    public class smartFanContext : DbContext
    {
        public smartFanContext(DbContextOptions<smartFanContext> options)
            : base(options) { }

        public DbSet<TemperatureLog> TemperatureLogs { get; set; }
        public DbSet<DeviceConfig> DeviceConfigs { get; set; }
        public DbSet<ManualOverride> ManualOverrides { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //One-to-Many: DeviceConfig --> TemperatureLogs
            modelBuilder.Entity<DeviceConfig>()
                .HasMany(dc => dc.TemperatureLogs)
                .WithOne(tl => tl.DeviceConfig)
                .HasForeignKey(tl => tl.DeviceConfigId)
                .OnDelete(DeleteBehavior.Cascade);

            //One-to-One: DeviceConfig --> ManualOverride
            modelBuilder.Entity<DeviceConfig>()
                .HasOne(dc => dc.ManualOverride)
                .WithOne(mo => mo.DeviceConfig)
                .HasForeignKey<ManualOverride>(mo => mo.DeviceConfigId);

            //One-to-Many: DeviceConfig --> ErrorLogs
            modelBuilder.Entity<DeviceConfig>()
                .HasMany(dc => dc.ErrorLogs)
                .WithOne(el => el.DeviceConfig)
                .HasForeignKey(el => el.DeviceConfigId)
                .OnDelete(DeleteBehavior.Cascade);
        }
   }
}