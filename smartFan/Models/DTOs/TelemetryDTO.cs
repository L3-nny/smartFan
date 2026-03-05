using System;
using System.Text.Json.Serialization;

namespace smartFan.Models.DTOs
{
    /// <summary>
    /// Represents the data coming from the ESP32 to the Server
    /// </summary>
    public class TelemetryModel
    {
        // "t for temperature"
        [JsonPropertyName("t")]
        public double Temperature { get; set; }
        
        // "h for humidity"
        // [JsonPropertyName("h")]
        // public double Humidity { get; set; }

        //"d" for Device ID
        // [JsonPropertyName("d")]
        // public int DeviceId { get; set;}

    }
}
