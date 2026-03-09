#include "fan_controller.h"
#include "config.h"

void FanController::init() {
    
    pinMode(MOSFET_PIN, OUTPUT); // Pin 13
    analogWrite(MOSFET_PIN, 0);
}

void FanController::updateState(int speed, int mode) {

    int pwmValue = 0;
    switch (speed) {
        case 1: pwmValue = 85; break; 
        case 2: pwmValue = 170; break;
        case 3: pwmValue = 255; break; 
        default: pwmValue = 0; break; 
    }

    // SMOKING GUN: If this doesn't print, updateState isn't being called
    //REMOVE IN PROD
    Serial.printf("[Hardware] Writing PWM %d to Pin %d\n", pwmValue, MOSFET_PIN);

    analogWrite(MOSFET_PIN, pwmValue);

    //Visual feedback for manual mode
    if (mode == 1) {
        digitalWrite(STATUS_LED_PIN, !digitalRead(STATUS_LED_PIN)); 
    } else {
        digitalWrite(STATUS_LED_PIN, HIGH);
    }
}

void FanController::setEmergencySpeed() {
    analogWrite(MOSFET_PIN, 85);
    digitalWrite(STATUS_LED_PIN, LOW); //Off to indicate error
}