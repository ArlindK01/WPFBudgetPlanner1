using WPFBudgetPlanner.Models;

namespace WPFBudgetPlanner.Services;

public interface ISalaryCalculator
{
    decimal GetMonthlyIncome(UserSetting settings);
    decimal GetHourlyRate(UserSetting settings);
}