//Include the necessary libraries
#include <Arduino.h>
#include <WiFi.h>
#include <HTTPClient.h>
#include <ArduinoJson.h>
#include <config.h>
#include "thermal_math.h"

unsigned long lastUpdateTime = 0;
unsigned long lastWifiRetry = 0;

void setup() {
    Serial.begin(115200);


    pinMode(STATUS_LED_PIN, OUTPUT);
    // Initialize the PWM for the MOSFET control pin
    ledcSetup(0, PWM_FREQ, 8); 
    ledcAttachPin(MOSFET_PIN, 0);
    // Connect to Wi-Fi
    WiFi.begin(WIFI_SSID, WIFI_PASSWORD);
    lastWifiRetry = millis();
}

void loop() {
    unsigned long currentTime = millis();

    //Check connection status
    if (WiFi.status() != WL_CONNECTED && (currentTime - lastWifiRetry) >= 30000) {
        Serial.println("Retriggering WiFi connection...");
        WiFi.disconnect();
        WiFi.begin(WIFI_SSID, WIFI_PASSWORD);
        lastWifiRetry = currentTime;
    
    
        }
    }

void loop() {
    unsigned long currentTime = millis();

    //Fetch the temperature data every 5 seconds
    if (WiFi.status() == WL_CONNECTED && (currentTime - lastUpdateTime) >= 5000) {
        int rawValue = analogRead(THERMISTOR_PIN);
        float celsius = ThermalMath::calculateCelsius(rawValue);
        Serial.print("Temperature: celsius);");

        //Create JSON payload
        StaticJsonDocument<128> doc;
        doc["t"] = celsius;
        String jsonPayload;
        serializeJson(doc, jsonPayload);
        
        //Send json payload to  the server
        HTTPClient http;
        http.begin(SERVER_URL);
        http.addHeader("Content-Type", "application/json");
        int httpResponseCode = http.POST(jsonPayload);
    }
}