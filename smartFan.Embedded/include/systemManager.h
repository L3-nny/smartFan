#ifndef SYSTEM_MANAGER_H
#define SYSTEM_MANAGER_H

#include "gateway_client.h"
#include "fan_controller.h"

class SystemManager {
public:
    void setup();
    void update();

private:
    GatewayClient _gateway;
    FanController _fan;
    
    unsigned long _lastUpdate = 0;
    const unsigned long _interval = 4500; 

    void _processTick(); 
};

#endif