# Smart Fan Blazor UI Dashboard

A modern, responsive web dashboard for monitoring and controlling the Smart Fan system built with Blazor WebAssembly and TailwindCSS.

## Features

### 🎛️ Real-time Dashboard
- Live temperature monitoring with auto-refresh every 5 seconds
- Fan speed display with visual RPM indicators
- Online/offline device status with animated indicators
- Automatic/manual mode switching

### 📊 Interactive Components
- **Device Status Card**: Shows temperature, fan speed, mode, and device health
- **Manual Override Panel**: 4 large buttons (OFF, LOW, MEDIUM, HIGH) for manual control
- **Fan Control Gauge**: Circular gauge visualization with color-coded speed levels
- **Auto-refresh Toggle**: Enable/disable automatic data updates

### 🎨 Modern UI Design
- Clean, professional interface matching Figma design specifications
- TailwindCSS responsive layout that works on desktop and mobile
- Smooth animations and transitions
- Loading states and error handling with user-friendly messages
- Toast notifications for errors and status updates

## Architecture

### 📁 Project Structure
```
smartFan.BlazorUI/
├── Components/           # Reusable Blazor components
│   ├── DeviceStatusCard.razor
│   ├── ManualOverridePanel.razor
│   └── FanControlGauge.razor
├── Models/              # Data models and DTOs
│   └── DeviceStatus.cs
├── Pages/               # Page components
│   ├── Dashboard.razor  # Main dashboard page
│   └── Home.razor      # Redirect to dashboard
├── Services/            # HTTP API services
│   ├── IServices.cs    # Service interfaces
│   ├── TemperatureService.cs
│   ├── ActuatorService.cs
│   └── DeviceService.cs
└── wwwroot/            # Static assets
    ├── css/app.css     # TailwindCSS imports
    └── appsettings.json # API configuration
```

### 🔧 Service Layer
- **ITemperatureService**: Gets current temperature readings
- **IActuatorService**: Controls fan speed and manual overrides
- **IDeviceService**: Aggregates device status from multiple sources
- **HttpClient**: Configured to connect to Smart Fan backend API

### 📡 API Integration
Connects to the Smart Fan ASP.NET Core backend endpoints:
- `GET /api/Temperature/current` - Current temperature reading
- `GET /api/Actuator/speed` - Current fan speed
- `POST /api/Actuator/adjust` - Adjust fan speed (manual override)

## Configuration

### API Base URL
Update the API base URL in `wwwroot/appsettings.json`:
```json
{
  "ApiBaseUrl": "http://localhost:5000/"
}
```

### Backend Requirements
The Blazor UI requires the Smart Fan backend API to be running with:
- Temperature endpoint returning `{ Temperature: double, Unit: string, Timestamp: datetime }`
- Actuator endpoints for fan speed control
- CORS enabled for the Blazor app domain

## Usage

### 🚀 Running the Application
1. Start the Smart Fan backend API
2. Run the Blazor app:
   ```bash
   cd smartFan.BlazorUI
   dotnet run
   ```
3. Navigate to `https://localhost:5001` (or the displayed URL)

### 🎮 Using the Dashboard
1. **Monitor**: View real-time temperature and fan speed data
2. **Manual Control**: Click any fan speed button (OFF/LOW/MEDIUM/HIGH) to override automatic control
3. **Auto-refresh**: Toggle automatic updates on/off as needed
4. **Status**: Green dot = online, red dot = offline/error

### 📱 Responsive Design
- **Desktop**: Full 2-column layout with device card and gauge side-by-side
- **Mobile**: Stacked layout with components optimized for touch interaction
- **Tablet**: Responsive grid that adapts to screen size

## Technical Details

### 🔄 Real-time Updates
- Timer-based refresh every 5 seconds (configurable)
- Immediate UI updates on manual fan speed changes
- Graceful error handling with retry logic
- Loading states during API calls

### 🎨 UI Components
- **TailwindCSS**: Modern utility-first CSS framework
- **Custom animations**: Pulse effects, loading spinners, transitions
- **SVG icons**: Scalable icons for better performance
- **Color coding**: 
  - Gray: OFF
  - Blue: LOW
  - Orange: MEDIUM  
  - Red: HIGH

### 🛡️ Error Handling
- Network connectivity errors with user-friendly messages
- API timeout handling with retry mechanisms
- Graceful degradation when backend is unavailable
- Auto-clearing error notifications

### 🔧 Performance
- Blazor WebAssembly for fast client-side rendering
- Minimal API calls with intelligent caching
- Lazy loading and code splitting
- Optimized bundle size with tree shaking

## Development

### Prerequisites
- .NET 8.0 SDK or later
- Smart Fan backend API running
- Web browser with WebAssembly support

### Building
```bash
dotnet build
```

### Running in Development
```bash
dotnet run --environment Development
```

### Deployment
```bash
dotnet publish -c Release
```

Deploy the contents of `bin/Release/net8.0/publish/wwwroot/` to any static web server.

## Troubleshooting

### Common Issues
1. **"Device Offline"**: Check that the backend API is running and accessible
2. **CORS errors**: Ensure backend allows requests from Blazor app domain
3. **API not found**: Verify the ApiBaseUrl in appsettings.json
4. **Styling issues**: Check that TailwindCSS CDN is loading properly

### Debugging
- Open browser developer tools to see console logs
- Check Network tab for failed API requests
- Blazor logging is enabled in Development mode