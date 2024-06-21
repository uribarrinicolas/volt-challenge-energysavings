using System.Text.Json.Serialization;

namespace EnergySavings.Models;

public class EnergyReportModel
{
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
    [JsonPropertyName("external_network_electricity_consumption")]
    public double ExternalNetworkElectricityConsumption { get; set; }
    [JsonPropertyName("savings")]
    public double Savings { get; set; }
}
