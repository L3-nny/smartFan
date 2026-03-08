#include "fan_controller.h"
#include "config.h"

void FanController::init() {
    ledcSetup(0, PWM_FREQ, 8);
    ledcAttachPin(MOSFET_PIN, 0);
}

void FanController::setSpeed(int level) {
    int pwmValue = 0;
    switch (level) {
        case 1: pwmValue = 85; break; 
        case 2: pwmValue = 170; break;
        case 3: pwmValue = 255; break; 
        default: pwmValue = 0; break; 
    }
    ledcWrite(0, pwmValue);}