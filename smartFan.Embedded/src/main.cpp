#include <Arduino.h>
#include "systemManager.h"


SystemManager fanSystem;

void setup() {
    Serial.begin(115200);
    fanSystem.setup();
}

void loop() {
    fanSystem.update();
}