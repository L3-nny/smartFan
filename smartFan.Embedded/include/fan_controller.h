#ifndef FAN_CONTROLLER_H
#define FAN_CONTROLLER_H

#include <Arduino.h>

class FanController {
    public:
        void init();
        void updateState(int speed, int mode);
        void setEmergencySpeed();

};

#endif
