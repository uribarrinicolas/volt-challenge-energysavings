using EnergySavings.Helpers;
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

        var householdConsumption = await FileHelper.ReadJsonFileAsync<List<HouseholdConsumptionModel>>(consumptionFilePath);
        var solarOutput = await FileHelper.ReadJsonFileAsync<List<SolarOutputModel>>(solarOutputFilePath);

        Console.WriteLine($"TS={householdConsumption[600].Timestamp};EC={householdConsumption[600].ElectricityConsumption}kw;");
        Console.WriteLine($"TS={solarOutput[600].Timestamp};SO={solarOutput[600].SolarOutput}kw;");

        var report = EnergyReportService.GenerateEnergyReport(householdConsumption, solarOutput);

        Console.WriteLine($"TS={report[600].Timestamp};ENEC={report[600].ExternalNetworkElectricityConsumption}kw;S=${report[600].Savings};");

        await FileHelper.WriteJsonFileAsync(reportFilePath, report);
        Console.WriteLine("Finishing");

    }
}
