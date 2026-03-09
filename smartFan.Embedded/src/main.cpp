#include <Arduino.h>
#include "systemManager.h"


SystemManager fanSystem;

void setup() {
    Serial.begin(115200);
    fanSystem.setup();

    // Debug output to confirm WiFi connection and IP address
    //REMOVE IN PROD; MAYBE
    Serial.print("\n[Network] ESP32 IP: ");
    Serial.println(WiFi.localIP());
}

void loop() {
    fanSystem.update();
}