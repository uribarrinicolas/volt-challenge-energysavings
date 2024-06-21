using System.Text.Json;
using EnergySavings.Models;

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

        var report = GenerateEnergyReport(householdConsumption, solarOutput);

        Console.WriteLine($"TS={report[600].Timestamp};ENEC={report[600].ExternalNetworkElectricityConsumption}kw;S=${report[600].Savings};");

        SaveReportToJson(report, reportFilePath);
        Console.WriteLine("Finishing");

    }

    private static async Task<T> ReadJsonFileAsync<T>(string filePath)
    {
        using (var stream = File.OpenRead(filePath))
        {
            return await JsonSerializer.DeserializeAsync<T>(stream);
        }
    }

    private static void SaveReportToJson(List<EnergyReportModel> report, string filePath)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        };

        var jsonString = JsonSerializer.Serialize(report, options);
        File.WriteAllText(filePath, jsonString);
    }

    private static List<EnergyReportModel> GenerateEnergyReport(List<HouseholdConsumptionModel> consumption, List<SolarOutputModel> solarOutput)
    {
        var report = new List<EnergyReportModel>();

        for (int i = 0; i < consumption.Count; i++)
        {
            var timestamp = consumption[i].Timestamp;
            var electricityConsumption = consumption[i].ElectricityConsumption;
            var solarEnergy = solarOutput[i].SolarOutput;
            var externalNetworkElectricityConsumption = electricityConsumption - solarEnergy;
            var savings = solarEnergy * GetKwhCost(timestamp) / 60;

            report.Add(new EnergyReportModel
            {
                Timestamp = timestamp,
                ExternalNetworkElectricityConsumption = externalNetworkElectricityConsumption,
                Savings = savings
            });
        }

        return report;
    }

    private static double GetKwhCost(DateTime timestamp)
    {
        double baseCost = ObtainBaseCost(timestamp);
        double finalCost = ApplyWinterFee(baseCost, timestamp);

        return finalCost;
    }

    private static double ObtainBaseCost(DateTime timestamp)
    {
        bool isWeekend = timestamp.DayOfWeek == DayOfWeek.Saturday || timestamp.DayOfWeek == DayOfWeek.Sunday;
        int hour = timestamp.Hour;

        if (isWeekend)
        {
            if (hour >= 0 && hour < 9)
                return 0.12;
            else if (hour >= 9 && hour < 17)
                return 0.10;
            else
                return 0.11;
        }
        else
        {
            if (hour >= 0 && hour < 6)
                return 0.08;
            else if (hour >= 6 && hour < 9)
                return 0.15;
            else if (hour >= 9 && hour < 14)
                return 0.10;
            else if (hour >= 14 && hour < 17)
                return 0.11;
            else if (hour >= 17 && hour < 22)
                return 0.13;
            else
                return 0.09;
        }
    }

    private static double ApplyWinterFee(double prev, DateTime timestamp)
    {
        if (timestamp.Month == 12 || timestamp.Month == 1 || timestamp.Month == 2)
        {
            return prev * 1.15;
        }
        return prev;
    }
}
