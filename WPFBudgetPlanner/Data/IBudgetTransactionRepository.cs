using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WPFBudgetPlanner.Models;

namespace WPFBudgetPlanner.Data;

public interface IBudgetTransactionRepository
{
    Task<BudgetTransaction?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<BudgetTransaction>> GetByMonthAsync(int year, int month);
    Task<IReadOnlyList<BudgetTransaction>> GetActiveAsync();
    Task AddAsync(BudgetTransaction transaction);
    Task UpdateAsync(BudgetTransaction transaction);
    Task DeleteAsync(Guid id);
    Task<int> SaveChangesAsync();
}