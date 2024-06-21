namespace EnergySavings.Services;

public class ElectricityCostService
{
    public static double GetKwhCost(DateTime timestamp)
    {
        double baseCost = ObtainBaseCost(timestamp);
        double finalCost = ApplyWinterFee(baseCost, timestamp);

        return finalCost;
    }

    private static double ObtainBaseCost(DateTime timestamp)
    {
        bool isWeekend = timestamp.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
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

    private static double ApplyWinterFee(double prev, DateTime timestamp) => timestamp.Month switch
    {
        12 or 1 or 2 => prev * 1.15,
        _ => prev
    };
}