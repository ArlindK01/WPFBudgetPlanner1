using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WPFBudgetPlanner.Models;

namespace WPFBudgetPlanner.Data;

public sealed class BudgetTransactionRepository : IBudgetTransactionRepository
{
    private readonly IDbContextFactory<BudgetDbContext> _factory;

    public BudgetTransactionRepository(IDbContextFactory<BudgetDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<BudgetTransaction?> GetByIdAsync(Guid id)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.BudgetTransactions.FindAsync(id);
    }

    public async Task<IReadOnlyList<BudgetTransaction>> GetByMonthAsync(int year, int month)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var start = new DateTime(year, month, 1);
        var end = start.AddMonths(1);

        return await db.BudgetTransactions
            .AsNoTracking()
            .Where(t => t.IsActive && (
                (t.RecurrenceType == RecurrenceType.None && ((t.Date >= start && t.Date < end) || (t.RecurrenceMonth == month))) ||
                (t.RecurrenceType == RecurrenceType.Monthly) ||
                (t.RecurrenceType == RecurrenceType.Yearly && t.RecurrenceMonth == month)
            ))
            .OrderBy(t => t.Date)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<BudgetTransaction>> GetActiveAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.BudgetTransactions
            .AsNoTracking()
            .Where(t => t.IsActive)
            .ToListAsync();
    }

    public async Task AddAsync(BudgetTransaction transaction)
    {
        await using var db = await _factory.CreateDbContextAsync();
        transaction.CreatedAt = DateTime.UtcNow;
        await db.BudgetTransactions.AddAsync(transaction);
        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(BudgetTransaction transaction)
    {
        await using var db = await _factory.CreateDbContextAsync();
        transaction.UpdatedAt = DateTime.UtcNow;
        db.BudgetTransactions.Update(transaction);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var entity = await db.BudgetTransactions.FindAsync(id);
        if (entity != null)
        {
            db.BudgetTransactions.Remove(entity);
            await db.SaveChangesAsync();
        }
    }

    public Task<int> SaveChangesAsync()
    {
        return Task.FromResult(0);
    }
}