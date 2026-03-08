#include "fan_controller.h"
#include "config.h"

void FanController::init() {
    ledcSetup(0, PWM_FREQ, 8);
    ledcAttachPin(MOSFET_PIN, 0);
}

void FanController::updateState(int speed, int mode) {

    int pwmValue = 0;
    switch (speed) {
        case 1: pwmValue = 85; break; 
        case 2: pwmValue = 170; break;
        case 3: pwmValue = 255; break; 
        default: pwmValue = 0; break; 
    }

    ledcWrite(0, pwmValue);

    //Visual feedback for manual mode
    if (mode == 1) {
        digitalWrite(STATUS_LED_PIN, !digitalRead(STATUS_LED_PIN)); 
    } else {
        digitalWrite(STATUS_LED_PIN, HIGH);
    }
}

void FanController::setEmergencySpeed() {
    ledcWrite(0, 85);
    digitalWrite(STATUS_LED_PIN, LOW); //Off to indicate error
}