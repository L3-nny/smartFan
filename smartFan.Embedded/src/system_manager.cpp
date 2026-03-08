#include "systemManager.h"
#include "config.h"
#include "thermal_math.h"
#include <Arduino.h>


void SystemManager::setup() {
    _fan.init();
    _gateway.init();

    _lastUpdated = millis();

    Serial.println("[System] Boot sequence complete");
}

void SystemManager::update() {
    _gateway.maintainConnection();

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