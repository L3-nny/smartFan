using smartFan.Models;
using smartFan.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace smartFan.Services
{
    public class ActuatorService : IActuatorService
    {
        public FanSpeed CurrentSpeed { get; private set; }
        
        private readonly FanSettingsModel _fanSettings;
        private bool _manualOverrideActive = false;
        private DateTime _manualOverrideExpiry;
        private readonly TimeSpan _overrideDuration = TimeSpan.FromMinutes(5);
        public ActuatorService(IOptions<FanSettingsModel> options)
        {
            _fanSettings = options.Value;

            CurrentSpeed = FanSpeed.Off; // Default to off  
    
        }

        public void Update(double temperature)
        {
            // Check for manual override expiration
            if (_manualOverrideActive && DateTime.UtcNow > _manualOverrideExpiry)
            {
                _manualOverrideActive = false;
                Console.WriteLine("[Actuator] Manual override expired, returning to automatic control");
            }

            // Skip automatic updates if manual override is active
            if (_manualOverrideActive)
            {
                Console.WriteLine($"[Actuator] Manual override active, ignoring temperature {temperature:F1}°C");
                return;
            }

            var previousSpeed = CurrentSpeed;

            if (temperature < _fanSettings.ThresholdOff)
            {
                CurrentSpeed = FanSpeed.Off;
            }
            else if (temperature < _fanSettings.ThresholdLow)
            {
                CurrentSpeed = FanSpeed.Low;
            }
            else if (temperature < _fanSettings.ThresholdMedium)
            {
                CurrentSpeed = FanSpeed.Medium;
            }
            else
            {
                CurrentSpeed = FanSpeed.High;
            }

            if (previousSpeed != CurrentSpeed)
            {
                Console.WriteLine($"[Actuator] Temp: {temperature:F1}°C --> Fan: {previousSpeed} → {CurrentSpeed}");
            }
        }

        /// <summary>
        /// Manually set fan speed with temporary override
        /// </summary>
        public void SetManualOverride(FanSpeed speed)
        {
            CurrentSpeed = speed;
            _manualOverrideActive = true;
            _manualOverrideExpiry = DateTime.UtcNow.Add(_overrideDuration);
            Console.WriteLine($"[Actuator] Manual override set: {speed} (expires in {_overrideDuration.TotalMinutes} minutes)");
        }

        /// <summary>
        /// Clear manual override and return to automatic control
        /// </summary>
        public void ClearManualOverride()
        {
            _manualOverrideActive = false;
            Console.WriteLine("[Actuator] Manual override cleared, automatic control resumed");
        }

        /// <summary>
        /// Check if manual override is currently active
        /// </summary>
        public bool IsManualOverrideActive => _manualOverrideActive && DateTime.UtcNow <= _manualOverrideExpiry;
    }
}