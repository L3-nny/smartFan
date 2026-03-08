#include <Arduino.h>
#include <WiFi.h>
#include <HTTPClient.h>
#include <ArduinoJson.h>
#include "config.h"
#include "thermal_math.h"
#include "fan_controller.h"

// Instantiate the fan controller
FanController fan;

unsigned long lastUpdateTime = 0;
unsigned long lastWifiRetry = 0;

void setup() {
    Serial.begin(115200);
    
    // Initialize Fan Hardware (PWM Setup)
    fan.init();

    pinMode(STATUS_LED_PIN, OUTPUT);

    // Connect to Wi-Fi
    WiFi.begin(WIFI_SSID, WIFI_PASSWORD);
    lastWifiRetry = millis();
}

void loop() {
    unsigned long currentTime = millis();

    // 1. WiFi Maintenance: Check connection status every 30s if disconnected
    if (WiFi.status() != WL_CONNECTED && (currentTime - lastWifiRetry) >= 30000) {
        Serial.println("Retriggering WiFi connection...");
        WiFi.disconnect();
        WiFi.begin(WIFI_SSID, WIFI_PASSWORD);
        lastWifiRetry = currentTime;
    }

    // 2. Data Pipeline: Fetch and send data every 5 seconds
    if (WiFi.status() == WL_CONNECTED && (currentTime - lastUpdateTime) >= 5000) {
        
        int rawValue = analogRead(THERMISTOR_PIN);
        float celsius = ThermalMath::calculateCelsius(rawValue);

        Serial.print("Temperature: "); 
        Serial.println(celsius);

        // Create JSON payload
        JsonDocument doc;
        doc["t"] = celsius;
        String jsonPayload;
        serializeJson(doc, jsonPayload);
        
        // Send json payload to the server
        HTTPClient http;
        http.begin(SERVER_URL);

        //Set timeout
        http.setTimeout(4500);

        http.addHeader("Content-Type", "application/json");
        int httpResponseCode = http.POST(jsonPayload);

        // Handle the response
        if (httpResponseCode > 0) {
            String response = http.getString();
            Serial.println("Server Response: " + response);

            JsonDocument responseDoc;
            DeserializationError error = deserializeJson(responseDoc, response);
        
            if (!error) {
                int speed = responseDoc["s"];
                int mode = responseDoc["m"];
                Serial.printf("New fan speed: %d, Mode: %d\n", speed, mode);
                
                // Update Hardware
                fan.setSpeed(speed);
            }
        } 
        else {
            // Log network/server errors
            Serial.printf("HTTP Error: %d - %s\n", 
                          httpResponseCode, 
                          http.errorToString(httpResponseCode).c_str());
        }

        // Clean up resources and reset timer AFTER work is done
        http.end(); 
        lastUpdateTime = millis(); 
    }
}