using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WPFBudgetPlanner.Models;

namespace WPFBudgetPlanner.Data;

public sealed class UserSettingRepository : IUserSettingRepository
{
    private readonly IDbContextFactory<BudgetDbContext> _factory;

    public UserSettingRepository(IDbContextFactory<BudgetDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<UserSetting?> GetAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.UserSettings
            .AsNoTracking()
            .OrderByDescending(s => s.UpdatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task UpsertAsync(UserSetting settings)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var existing = await db.UserSettings.FirstOrDefaultAsync();
        var now = DateTime.UtcNow;
        if (existing == null)
        {
            settings.UpdatedAt = now;
            await db.UserSettings.AddAsync(settings);
        }
        else
        {
            existing.AnnualIncome = settings.AnnualIncome;
            existing.AnnualWorkHours = settings.AnnualWorkHours;
            existing.Currency = settings.Currency;
            existing.UpdatedAt = now;
            db.UserSettings.Update(existing);
        }
        await db.SaveChangesAsync();
    }

    public Task<int> SaveChangesAsync()
    {
        return Task.FromResult(0);
    }
}