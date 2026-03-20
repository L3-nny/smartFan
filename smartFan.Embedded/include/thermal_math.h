#ifndef THERMAL_MATH_H
#define THERMAL_MATH_H

#include <math.h>

class ThermalMath {
public:
// Constants for the Steinhart-Hart equation
static constexpr double A = 0.001129148;
static constexpr double B = 0.000234125;
static constexpr double C = 0.0000000876741;

//Resistance of the balance resistor(10kΩ)
static constexpr double R_Balance = 10000.0;

static double calculateCelsius(int rawValue) {
    if (rawValue <= 0 || rawValue >= 4095) 
        return 0.0;

    
    double dynamic_resistance = R_Balance / ((4095.0 / (double)rawValue) - 1.0);

    //Steinhart-Hart equation
    double log_R = log(dynamic_resistance);
    double param_c = log_R * log_R * log_R;
    double temperature = 1.0 / (A + (B * log_R) + (C * param_c)); 

    return temperature - 273.15;
}

};

#endif