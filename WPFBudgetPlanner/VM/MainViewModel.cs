using System.ComponentModel;
using System.Threading.Tasks;
using WPFBudgetPlanner.Data;
using WPFBudgetPlanner.Models;
using WPFBudgetPlanner.Services;
using WPFBudgetPlanner.VM.Overview;
using WPFBudgetPlanner.VM.Transactions;

namespace WPFBudgetPlanner.VM;

public sealed class MainViewModel : ViewModelBase
{
    private readonly IBudgetTransactionRepository _transactionRepository;
    private readonly IUserSettingRepository _userSettingRepository;
    private readonly IBudgetForecastService _forecastService;
    private readonly ISalaryCalculator _salaryCalculator;

    private BudgetTransactionListViewModel? _transactions;
    private BudgetViewModel? _budget;
    private UserSetting? _settings;

    public MainViewModel(
        IBudgetTransactionRepository transactionRepository,
        IUserSettingRepository userSettingRepository,
        IBudgetForecastService forecastService,
        ISalaryCalculator salaryCalculator,
        BudgetTransactionListViewModel transactionsViewModel,
        BudgetViewModel budgetViewModel)
    {
        _transactionRepository = transactionRepository;
        _userSettingRepository = userSettingRepository;
        _forecastService = forecastService;
        _salaryCalculator = salaryCalculator;

        Transactions = transactionsViewModel;
        Budget = budgetViewModel;

        if (Transactions != null)
        {
            Transactions.TransactionsChanged += OnTransactionsChanged;
        }
        if (Budget != null)
        {
            Budget.PropertyChanged += OnBudgetPropertyChanged;
        }
    }

    public BudgetTransactionListViewModel? Transactions
    {
        get => _transactions;
        set
        {
            if (!ReferenceEquals(_transactions, value))
            {
                if (_transactions != null)
                {
                    _transactions.TransactionsChanged -= OnTransactionsChanged;
                }
                _transactions = value;
                if (_transactions != null)
                {
                    _transactions.TransactionsChanged += OnTransactionsChanged;
                }
                RaisePropertyChanged();
            }
        }
    }

    public BudgetViewModel? Budget
    {
        get => _budget;
        set
        {
            if (!ReferenceEquals(_budget, value))
            {
                if (_budget != null)
                {
                    _budget.PropertyChanged -= OnBudgetPropertyChanged;
                }
                _budget = value;
                if (_budget != null)
                {
                    _budget.PropertyChanged += OnBudgetPropertyChanged;
                }
                RaisePropertyChanged();
            }
        }
    }

    public UserSetting? Settings
    {
        get => _settings;
        set
        {
            if (!ReferenceEquals(_settings, value))
            {
                _settings = value;
                RaisePropertyChanged();
            }
        }
    }

    public async Task InitializeAsync()
    {
        if (Budget != null)
        {
            var year = Budget.SelectedMonth.Year;
            var month = Budget.SelectedMonth.Month;
            await Budget.RefreshAsync(year, month);
            if (Transactions != null)
            {
                await Transactions.LoadAsync(year, month);
            }
        }
    }

    private async void OnTransactionsChanged()
    {
        if (Budget != null && Transactions != null)
        {
            var year = Budget.SelectedMonth.Year;
            var month = Budget.SelectedMonth.Month;
            await Budget.RefreshAsync(year, month);
            await Transactions.LoadAsync(year, month);
        }
    }

    private async void OnBudgetPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Overview.BudgetViewModel.SelectedMonth) && Budget != null)
        {
            var year = Budget.SelectedMonth.Year;
            var month = Budget.SelectedMonth.Month;
            await Budget.RefreshAsync(year, month);
            if (Transactions != null)
            {
                await Transactions.LoadAsync(year, month);
            }
        }
    }
}