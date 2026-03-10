# smartFan Gateway

A high-performance IoT gateway designed to bridge physical ESP32 hardware with intelligent fan control logic.

## System Architecture

The system follows a robust, event-driven IoT loop that bridges a physical C++ environment with a .NET ecosystem. At the edge, the ESP32 microcontroller acts as the hardware interface, sampling live data from a thermistor and executing variable fan speeds via PWM through an **IRL520N MOSFET**.

Data is transmitted over the local network to a [`GatewayController`](./smartFan/Controllers/GatewayController.cs) within an **ASP.NET Core Web API**, which serves as the bridge between the physical device and the internal logic. Once a temperature reading enters the pipeline, it is processed by an **ActuatorService**, which evaluates the reading against predefined thresholds to determine the optimal fan state.

This state is simultaneously broadcast in real-time to a [`Blazor WebAssembly Dashboard`](./smartFan.BlazorUI/Pages/). The loop completes when the ESP32 polls the Gateway for its next command, receiving a lightweight integer status that ensures the physical fan speed and the UI status indicator remain perfectly synchronized.

## Hardware Setup

The physical layer relies on a precise voltage divider circuit for temperature acquisition and a logic level MOSFET for high current fan control. 

*(Place your schematic image below)*



## Getting Started

### Installation and Setup

**Prerequisites:** .NET 8.0 SDK or later

1. **Clone the repository:**
   ```bash
   git clone https://github.com/L3-nny/smartFan.git
   ```
2. **Build the project:**
   ```bash
   dotnet build
   ```
3. **Run the application:**
   ```bash
   dotnet run
   ```

### Configuration

Temperature thresholds are managed via [`appsettings.json`](./smartFan/appsettings.json) on the server. However, to maintain system stability during periods of high network latency or outages, the ESP32 firmware includes local safety overrides. Ensure both the server-side thresholds and the firmware's failsafe logic are synchronized during deployment.

## API Documentation

### Endpoint Reference

#### **`POST` /api/telemetry**

Primary communication hub between the ESP32 and the .NET backend. The ESP32 sends real-time sensor data and receives the calculated fan state in the response.

**Request Body (JSON)**
```json
{
    "t": 25.5,
    "d": 1
}
```
* **`t`**: The current temperature in Celsius, calculated using the [`ThermalMath`](./smartFan.Embedded/include/thermal_math.h) class.
* **`d`**: Unique device ID used to identify the specific hardware unit, mapped to the [`TelemetryDTO`](./smartFan/Models/DTOs/TelemetryDTO.cs)

**Success Response (200 OK)**
```json
{
    "s": 2,
    "m": 0
}
```
* **`s`**: Target fan speed level (0: Off, 1: Low, 2: Medium, 3: High).
* **`m`**: Operational mode (0: Auto, 1: Manual Override).

## Development and Testing

### Swagger UI

For manual endpoint testing and documentation, Swagger is available in the **Development** environment.
* **URL:** `http://<your-pc-ip>:<port>/swagger` (e.g., `http://192.168.0.000:5000/swagger`)

> **Security Note:** Swagger is automatically disabled in Production mode to prevent exposure of the service's internal schema.

### Diagnostic Error Reference

| Code | Status | Description & Troubleshooting |
| :--- | :--- | :--- |
| **-1** | Connection Refused | The ESP32 cannot reach the server. Verify the `SERVER_URL` IP address and ensure the host firewall allows traffic on the target port. |
| **400** | Bad Request | Data mismatch. The JSON payload structure sent by the firmware does not match the .NET `TelemetryModel`. |
| **404** | Not Found | Endpoint unreachable. The API route is incorrect or the server is not listening on the specified path. |
| **500** | Internal Server Error | Server-side crash. An unhandled exception occurred within the .NET backend logic during processing. |

## Roadmap: Scaling with MQTT

While the current HTTP request-response architecture is highly effective for a localized prototype, accommodating a larger fleet of devices requires a shift in the networking protocol to maintain performance. 

Future iterations of this project will migrate the communication layer to **MQTT** to solve the following engineering constraints:

* **Eliminating Polling Latency:** Transitioning to a publish-subscribe model allows the .NET backend to push manual fan overrides instantly. The ESP32 will no longer need to wait for the next 5-second polling window to receive updated commands.
* **Reducing Network Overhead:** HTTP requires establishing a connection and transmitting bulky headers for every payload. MQTT maintains a persistent, lightweight connection, drastically reducing the bandwidth and memory footprint on the ESP32.
* **Native Connection Tracking:** Utilizing MQTT's "Last Will and Testament" (LWT) feature will provide the server with immediate, native notifications if a hardware unit drops off the network, replacing the need for manual timeout calculations.



## Open Discussion

This project is actively evolving. Whether you are interested in discussing the hardware implementation, reviewing the .NET architecture, or proposing optimizations for the C++ firmware, collaboration is welcome. 

Feel free to open an [`Issue`](https://github.com/L3-nny/smartFan/issues) or start a thread in the [`Discussions`](https://github.com/L3-nny/smartFan/discussions) tab of this repository to share insights, report bugs, or discuss the planned migration to MQTT.