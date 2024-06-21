using EnergySavings.Helpers;
using EnergySavings.Models;
using EnergySavings.Services;

namespace EnergySavings;
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Volt Home - Advanced .NET Challenge");

        if (args.Length < 3)
        {
            Console.WriteLine("Usage: EnergySavings <input_household_consumption.json> <input_solar_output.json> <output_report.json>");
            return;
        }
        Console.WriteLine("Starting process");

        var consumptionFilePath = args[0];
        var solarOutputFilePath = args[1];
        var reportFilePath = args[2];

        var householdConsumption = await FileHelper.ReadJsonFileAsync<List<HouseholdConsumptionModel>>(consumptionFilePath);
        var solarOutput = await FileHelper.ReadJsonFileAsync<List<SolarOutputModel>>(solarOutputFilePath);

        var report = EnergyReportService.GenerateEnergyReport(householdConsumption, solarOutput);

        await FileHelper.WriteJsonFileAsync(reportFilePath, report);
        Console.WriteLine("Finished");
    }
}
