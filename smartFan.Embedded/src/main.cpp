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
