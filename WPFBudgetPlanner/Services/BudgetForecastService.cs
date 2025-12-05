using System.Collections.Generic;
using System.Linq;
using WPFBudgetPlanner.Models;

namespace WPFBudgetPlanner.Services;

public sealed class BudgetForecastService : IBudgetForecastService
{
    private readonly ISalaryCalculator _salaryCalculator;

    public BudgetForecastService(ISalaryCalculator salaryCalculator)
    {
        _salaryCalculator = salaryCalculator;
    }

    public (decimal monthlyIncome, decimal totalIncome, decimal totalExpense, decimal net) GetForecastForMonth(
        IEnumerable<BudgetTransaction> transactions,
        UserSetting settings,
        int year,
        int month)
    {
        var monthlyIncome = _salaryCalculator.GetMonthlyIncome(settings);
        var totalIncome = transactions.Where(t => t.TransactionType == TransactionType.Income).Sum(t => t.Amount);
        var totalExpense = transactions.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => t.Amount);
        var net = monthlyIncome + totalIncome - totalExpense;
        return (monthlyIncome, totalIncome, totalExpense, net);
    }
}