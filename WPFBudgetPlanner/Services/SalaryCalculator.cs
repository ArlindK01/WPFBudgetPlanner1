using WPFBudgetPlanner.Models;

namespace WPFBudgetPlanner.Services;

public sealed class SalaryCalculator : ISalaryCalculator
{
    public decimal GetMonthlyIncome(UserSetting settings)
    {
        return settings.AnnualIncome / 12m;
    }

    public decimal GetHourlyRate(UserSetting settings)
    {
        if (settings.AnnualWorkHours <= 0m) return 0m;
        return settings.AnnualIncome / settings.AnnualWorkHours;
    }
}