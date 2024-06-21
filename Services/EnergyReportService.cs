using EnergySavings.Models;

namespace EnergySavings.Services;

public class EnergyReportService
{
    public static List<EnergyReportModel> GenerateEnergyReport(List<HouseholdConsumptionModel> consumption, List<SolarOutputModel> solarOutput)
    {
        var report = new List<EnergyReportModel>();

        for (int i = 0; i < consumption.Count; i++)
        {
            var timestamp = consumption[i].Timestamp;
            var electricityConsumption = consumption[i].ElectricityConsumption;
            var solarEnergy = solarOutput[i].SolarOutput;
            var externalNetworkElectricityConsumption = electricityConsumption - solarEnergy;
            var savings = solarEnergy * ElectricityCostService.GetKwhCost(timestamp) / 60;

            report.Add(new EnergyReportModel
            {
                Timestamp = timestamp,
                ExternalNetworkElectricityConsumption = externalNetworkElectricityConsumption,
                Savings = savings
            });
        }

        return report;
    }
}