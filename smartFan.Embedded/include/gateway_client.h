#ifndef GATEWAY_CLIENT_H
#define GATEWAY_CLIENT_H

#include <Arduino.h>
#include <WiFi.h>
#include <HTTPClient.h>
#include <ArduinoJson.h>

class GatewayClient {
public:
    // Structure to hold the server's response
    struct FanCommand {
        int speed;
        int mode;
        bool success;
    };

    void init();
    void maintainConnection();
    FanCommand sendTelemetry(float temperature);

private:
    unsigned long lastWifiRetry = 0;
    bool isFirstConnectionAttempt = true;
};

#endif