using System.Collections.Generic;
using WPFBudgetPlanner.Models;

namespace WPFBudgetPlanner.Services;

public interface IBudgetForecastService
{
    (decimal monthlyIncome, decimal totalIncome, decimal totalExpense, decimal net) GetForecastForMonth(
        IEnumerable<BudgetTransaction> transactions,
        UserSetting settings,
        int year,
        int month);
}