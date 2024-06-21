using System.Text.Json.Serialization;

namespace EnergySavings.Models;

public class HouseholdConsumptionModel
{
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
    [JsonPropertyName("electricity_consumption")]
    public double ElectricityConsumption { get; set; }
}
