#include <Arduino.h>
#include "systemManager.h"


SystemManager system;

void setup() {
    Serial.begin(115200);
    system.setup();
}

void loop() {
    system.update();
}