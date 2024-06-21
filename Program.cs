using System.Text.Json;
using EnergySavings.Models;
using EnergySavings.Services;

namespace EnergySavings;
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Volt Home - Advanced .NET Challenge");
        var consumptionFilePath = "miami_household_consumption_with_timestamps.json";
        var solarOutputFilePath = "miami_solar_output_with_timestamps.json";
        var reportFilePath = "energy_report.json";

        var householdConsumption = await ReadJsonFileAsync<List<HouseholdConsumptionModel>>(consumptionFilePath);
        var solarOutput = await ReadJsonFileAsync<List<SolarOutputModel>>(solarOutputFilePath);

        Console.WriteLine($"TS={householdConsumption[600].Timestamp};EC={householdConsumption[600].ElectricityConsumption}kw;");
        Console.WriteLine($"TS={solarOutput[600].Timestamp};SO={solarOutput[600].SolarOutput}kw;");

        var report = EnergyReportService.GenerateEnergyReport(householdConsumption, solarOutput);

        Console.WriteLine($"TS={report[600].Timestamp};ENEC={report[600].ExternalNetworkElectricityConsumption}kw;S=${report[600].Savings};");

        await WriteJsonFileAsync(reportFilePath, report);
        Console.WriteLine("Finishing");

    }

    private static async Task<T> ReadJsonFileAsync<T>(string filePath)
    {
        using (var stream = File.OpenRead(filePath))
        {
            return await JsonSerializer.DeserializeAsync<T>(stream);
        }
    }

    public static async Task WriteJsonFileAsync<T>(string filePath, T data)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        };

        var jsonString = JsonSerializer.Serialize(data, options);
        await File.WriteAllTextAsync(filePath, jsonString);
    }
}
