using System;

namespace WPFBudgetPlanner.Models;

public sealed class BudgetTransaction
{
    public Guid Id { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal Amount { get; set; }
    public Category Category { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public RecurrenceType RecurrenceType { get; set; }
    public int? RecurrenceMonth { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}