using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WPFBudgetPlanner.Models;

namespace WPFBudgetPlanner.Data;

public sealed class BudgetDbContext : DbContext
{
    public DbSet<BudgetTransaction> BudgetTransactions { get; set; } = null!;
    public DbSet<UserSetting> UserSettings { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            var connectionString = configuration.GetConnectionString("Default")
                ?? "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=WPFBudgetPlanner;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BudgetTransaction>()
            .Property(p => p.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<UserSetting>()
            .Property(p => p.AnnualIncome)
            .HasPrecision(18, 2);

        modelBuilder.Entity<UserSetting>()
            .Property(p => p.AnnualWorkHours)
            .HasPrecision(18, 2);

        modelBuilder.Entity<BudgetTransaction>()
            .HasIndex(p => p.Date);
        modelBuilder.Entity<BudgetTransaction>()
            .HasIndex(p => new { p.TransactionType, p.Category });
        modelBuilder.Entity<BudgetTransaction>()
            .HasIndex(p => p.IsActive);
        modelBuilder.Entity<UserSetting>()
            .HasIndex(p => p.UpdatedAt);
    }
}