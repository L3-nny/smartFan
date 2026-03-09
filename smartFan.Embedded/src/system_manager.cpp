#include "systemManager.h"
#include "config.h"
#include "thermal_math.h"


void SystemManager::setup() {
    _fan.init();
    _gateway.init();

    _lastUpdated = millis();

    Serial.println("[System] Boot sequence complete");
}

void SystemManager::update() {
    _gateway.maintainConnection();

    // This runs EVERY SINGLE MICROSECOND
    // You will see a wall of dots in your Serial Monitor
    //REMOVE IN PROD, it's just to show that the system is alive and not frozen while waiting for the next tick
    static unsigned long lastDot = 0;
    if (millis() - lastDot >= 100) { 
        Serial.print(".");
        lastDot = millis();
    }

    if (millis() - _lastUpdated >= _interval) {
        _lastUpdated = millis();
        _processTick();
    }
}

void SystemManager::_processTick() {
    int rawValue = analogRead(THERMISTOR_PIN);
    double celsius = ThermalMath::calculateCelsius(rawValue);
    // Debug output
    Serial.printf("[System] Telemetry: %.2f°C | Raw: %d\n", celsius, rawValue);
    
    // Send the temperature to .NET API and get the Speed back
    GatewayClient::FanCommand cmd = _gateway.sendTelemetry(celsius);

    if (cmd.success) {
        _fan.updateState(cmd.speed, cmd.mode);
    }else {
        Serial.println("[System] Failed to get command from gateway");
        _fan.setEmergencySpeed();
    }
}