using System;
using System.Threading.Tasks;
using WPFBudgetPlanner.Data;
using WPFBudgetPlanner.Models;
using WPFBudgetPlanner.Services;

namespace WPFBudgetPlanner.VM.Overview;

public sealed class BudgetViewModel : WPFBudgetPlanner.VM.ViewModelBase
{
    private readonly IBudgetTransactionRepository _transactionRepository;
    private readonly IUserSettingRepository _userSettingRepository;
    private readonly IBudgetForecastService _forecastService;

    private DateTime _selectedMonth;
    private decimal _monthlyIncome;
    private decimal _totalIncome;
    private decimal _totalExpense;
    private decimal _net;

    private decimal _annualIncome;
    private decimal _annualWorkHours;
    private string _currency = "SEK";

    public BudgetViewModel(IBudgetTransactionRepository transactionRepository, IUserSettingRepository userSettingRepository, IBudgetForecastService forecastService)
    {
        _transactionRepository = transactionRepository;
        _userSettingRepository = userSettingRepository;
        _forecastService = forecastService;
        SelectedMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
    }

    public DateTime SelectedMonth
    {
        get => _selectedMonth;
        set
        {
            if (_selectedMonth != value)
            {
                _selectedMonth = new DateTime(value.Year, value.Month, 1);
                RaisePropertyChanged();
                _ = RefreshAsync(_selectedMonth.Year, _selectedMonth.Month);
            }
        }
    }

    public decimal MonthlyIncome
    {
        get => _monthlyIncome;
        private set
        {
            if (_monthlyIncome != value)
            {
                _monthlyIncome = value;
                RaisePropertyChanged();
            }
        }
    }

    public decimal TotalIncome
    {
        get => _totalIncome;
        private set
        {
            if (_totalIncome != value)
            {
                _totalIncome = value;
                RaisePropertyChanged();
            }
        }
    }

    public decimal TotalExpense
    {
        get => _totalExpense;
        private set
        {
            if (_totalExpense != value)
            {
                _totalExpense = value;
                RaisePropertyChanged();
            }
        }
    }

    public decimal Net
    {
        get => _net;
        private set
        {
            if (_net != value)
            {
                _net = value;
                RaisePropertyChanged();
            }
        }
    }

    public decimal AnnualIncome
    {
        get => _annualIncome;
        set { if (_annualIncome != value) { _annualIncome = value; RaisePropertyChanged(); } }
    }

    public decimal AnnualWorkHours
    {
        get => _annualWorkHours;
        set { if (_annualWorkHours != value) { _annualWorkHours = value; RaisePropertyChanged(); } }
    }

    public string Currency
    {
        get => _currency;
        set { if (_currency != value) { _currency = value ?? "SEK"; RaisePropertyChanged(); } }
    }

    public async Task RefreshAsync(int year, int month)
    {
        var settings = await _userSettingRepository.GetAsync();
        if (settings == null)
        {
            settings = new UserSetting
            {
                AnnualIncome = 0m,
                AnnualWorkHours = 2080m,
                Currency = "SEK"
            };
            await _userSettingRepository.UpsertAsync(settings);
        }

        AnnualIncome = settings.AnnualIncome;
        AnnualWorkHours = settings.AnnualWorkHours;
        Currency = settings.Currency;

        var transactionsForMonth = await _transactionRepository.GetByMonthAsync(year, month);
        var forecast = _forecastService.GetForecastForMonth(transactionsForMonth, settings, year, month);

        MonthlyIncome = forecast.monthlyIncome;
        TotalIncome = forecast.totalIncome;
        TotalExpense = forecast.totalExpense;
        Net = forecast.net;
    }

    public async Task SaveSettingsAsync()
    {
        var newSettings = new UserSetting
        {
            AnnualIncome = AnnualIncome,
            AnnualWorkHours = AnnualWorkHours,
            Currency = Currency
        };
        await _userSettingRepository.UpsertAsync(newSettings);
        await RefreshAsync(SelectedMonth.Year, SelectedMonth.Month);
    }
}