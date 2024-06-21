using System.Text.Json.Serialization;

namespace EnergySavings.Models;

public class SolarOutputModel
{
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
    [JsonPropertyName("solar_output")]
    public double SolarOutput { get; set; }
}
