#include "gateway_client.h"
#include "config.h"


void GatewayClient::init() {
    WiFi.begin(WIFI_SSID, WIFI_PASSWORD);
    lastWifiRetry = millis();
}

void GatewayClient::maintainConnection() {
    unsigned long currentTime = millis();

    //RUn if 30s padded or first time after boot
    if ((WiFi.status() != WL_CONNECTED && (currentTime - lastWifiRetry) >= 30000) || (isFirstConnectionAttempt)) {
        Serial.println(isFirstConnectionAttempt ? "[WiFi] First boot connection..." : "[WiFi] Reconnecting...");

        WiFi.disconnect();
        WiFi.begin(WIFI_SSID, WIFI_PASSWORD);
        
        //wait for connection with a timeout
        int attempts = 0;
        while (WiFi.status() != WL_CONNECTED && attempts < 30) {
            delay(500);
            Serial.print(".");
            attempts++;
        }

        if (WiFi.status() == WL_CONNECTED) {
            Serial.println("\nWiFi reconnected! IP: ");
            Serial.println(WiFi.localIP());
        } else {
            Serial.println("\nFailed to reconnect WiFi.");
        }

        isFirstConnectionAttempt = false; // Reset the flag so the 30s timer takes over
        lastWifiRetry = millis();
    }
}

GatewayClient::FanCommand GatewayClient::sendTelemetry(float celsius) {
    FanCommand result = {0, 0, false};
    
    if (WiFi.status() != WL_CONNECTED) return result;

    JsonDocument doc;
    doc["t"] = celsius;
    
    String jsonPayload;
    serializeJson(doc, jsonPayload);

    HTTPClient http;
    http.begin(SERVER_URL);
    http.setTimeout(4500);
    http.addHeader("Content-Type", "application/json");

    int httpResponseCode = http.POST(jsonPayload);

    if (httpResponseCode > 0) {
        String response = http.getString();

        
        JsonDocument responseDoc;
        if (!deserializeJson(responseDoc, response)) {
            result.speed = responseDoc["s"];
            result.mode = responseDoc["m"];
            result.success = true;
        }
    } else {
        Serial.printf("HTTP Error: %d\n", httpResponseCode);
    }

    http.end();
    return result;
}