using EnergySavings.Models;

namespace EnergySavings.Services;

public class EnergyReportService
{
    public static List<EnergyReportModel> GenerateEnergyReport(List<HouseholdConsumptionModel> consumption, List<SolarOutputModel> solarOutput)
    {
        var report = new List<EnergyReportModel>();

        for (int i = 0; i < consumption.Count; i++)
        {
            report.Add(GenerateEnergyReportPerMinute(consumption[i], solarOutput[i]));
        }

        return report;
    }

    private static EnergyReportModel GenerateEnergyReportPerMinute(HouseholdConsumptionModel consumption, SolarOutputModel solarOutput)
    {
        var timestamp = consumption.Timestamp;
        var electricityConsumption = consumption.ElectricityConsumption;
        var solarEnergy = solarOutput.SolarOutput;
        var externalNetworkElectricityConsumption = electricityConsumption - solarEnergy;
        var savings = solarEnergy * ElectricityCostService.GetKwhCost(timestamp) / 60;

        return new EnergyReportModel
        {
            Timestamp = timestamp,
            ExternalNetworkElectricityConsumption = externalNetworkElectricityConsumption,
            Savings = savings
        };
    }
}