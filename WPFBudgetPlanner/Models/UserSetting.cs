using System;

namespace WPFBudgetPlanner.Models;

public sealed class UserSetting
{
    public int Id { get; set; }
    public decimal AnnualIncome { get; set; }
    public decimal AnnualWorkHours { get; set; }
    public string Currency { get; set; } = "SEK";
    public DateTime UpdatedAt { get; set; }
}