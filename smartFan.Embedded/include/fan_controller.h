#ifndef FAN_CONTROLLER_H
#define FAN_CONTROLLER_H

#include <Arduino.h>

class FanController {
    public:
        static void init();
        static void setSpeed(int level);
};

#endif
