using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFBudgetPlanner.Command;
using WPFBudgetPlanner.Data;
using WPFBudgetPlanner.Models;

namespace WPFBudgetPlanner.VM.Transactions;

public sealed class BudgetTransactionListViewModel : WPFBudgetPlanner.VM.ViewModelBase
{
    private readonly IBudgetTransactionRepository _repo;

    private BudgetTransactionItemViewModel? _selectedItem;
    private decimal _newAmount;
    private Category _newCategory;
    private string _newDescription = string.Empty;
    private RecurrenceType _newRecurrenceType;
    private int? _newRecurrenceMonth;
    private TransactionType? _filterTransactionType;
    private Category? _filterCategory;

    public event Action? TransactionsChanged;

    public BudgetTransactionListViewModel(IBudgetTransactionRepository repo)
    {
        _repo = repo;

        AddIncomeCommand = new DelegateCommand(async _ => await AddIncomeAsync());
        AddExpenseCommand = new DelegateCommand(async _ => await AddExpenseAsync());
        DeleteSelectedCommand = new DelegateCommand(async _ => await DeleteSelectedAsync(), _ => SelectedItem is not null);
        EditSelectedCommand = new DelegateCommand(_ => { if (SelectedItem is not null) { SelectedItem.IsEditing = true; UpdateCommandStates(); } }, _ => SelectedItem is not null);
        SaveEditCommand = new DelegateCommand(async _ => { await SaveEditAsync(); UpdateCommandStates(); }, _ => SelectedItem?.IsEditing == true);
        CancelEditCommand = new DelegateCommand(_ => { CancelEdit(); UpdateCommandStates(); }, _ => SelectedItem?.IsEditing == true);
        RefreshCommand = new DelegateCommand(async p =>
        {
            if (p is ValueTuple<int, int> ym)
            {
                await LoadAsync(ym.Item1, ym.Item2);
            }
        });
    }

    public ObservableCollection<BudgetTransactionItemViewModel> Items { get; set; } = new();

    public BudgetTransactionItemViewModel? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (!ReferenceEquals(_selectedItem, value))
            {
                _selectedItem = value;
                RaisePropertyChanged();
                (DeleteSelectedCommand as DelegateCommand)?.RaiseCanExecuteChanged();
                (EditSelectedCommand as DelegateCommand)?.RaiseCanExecuteChanged();
                (SaveEditCommand as DelegateCommand)?.RaiseCanExecuteChanged();
                (CancelEditCommand as DelegateCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    public decimal NewAmount
    {
        get => _newAmount;
        set
        {
            if (_newAmount != value)
            {
                _newAmount = value;
                RaisePropertyChanged();
            }
        }
    }

    public Category NewCategory
    {
        get => _newCategory;
        set
        {
            if (_newCategory != value)
            {
                _newCategory = value;
                RaisePropertyChanged();
            }
        }
    }

    public string NewDescription
    {
        get => _newDescription;
        set
        {
            if (_newDescription != value)
            {
                _newDescription = value ?? string.Empty;
                RaisePropertyChanged();
            }
        }
    }

    public RecurrenceType NewRecurrenceType
    {
        get => _newRecurrenceType;
        set
        {
            if (_newRecurrenceType != value)
            {
                _newRecurrenceType = value;
                RaisePropertyChanged();
            }
        }
    }

    public int? NewRecurrenceMonth
    {
        get => _newRecurrenceMonth;
        set
        {
            if (_newRecurrenceMonth != value)
            {
                _newRecurrenceMonth = value;
                RaisePropertyChanged();
            }
        }
    }

    public TransactionType? FilterTransactionType
    {
        get => _filterTransactionType;
        set
        {
            if (_filterTransactionType != value)
            {
                _filterTransactionType = value;
                RaisePropertyChanged();
            }
        }
    }

    public Category? FilterCategory
    {
        get => _filterCategory;
        set
        {
            if (_filterCategory != value)
            {
                _filterCategory = value;
                RaisePropertyChanged();
            }
        }
    }

    public ICommand? AddIncomeCommand { get; set; }
    public ICommand? AddExpenseCommand { get; set; }
    public ICommand? DeleteSelectedCommand { get; set; }
    public ICommand? EditSelectedCommand { get; set; }
    public ICommand? SaveEditCommand { get; set; }
    public ICommand? CancelEditCommand { get; set; }
    public ICommand? RefreshCommand { get; set; }

    public async Task LoadAsync(int year, int month)
    {
        var transactionsForMonth = await _repo.GetByMonthAsync(year, month);
        Items.Clear();
        foreach (var transaction in transactionsForMonth)
        {
            Items.Add(new BudgetTransactionItemViewModel(transaction));
        }
    }

    public async Task AddIncomeAsync()
    {
        await AddTransactionAsync(TransactionType.Income);
    }

    public async Task AddExpenseAsync()
    {
        await AddTransactionAsync(TransactionType.Expense);
    }

    private bool ValidateForAdd(TransactionType transactionType)
    {
        if (NewAmount <= 0m) return false;
        if (NewRecurrenceMonth.HasValue && (NewRecurrenceMonth.Value < 1 || NewRecurrenceMonth.Value > 12))
            return false;
        if (transactionType == TransactionType.Expense && string.IsNullOrWhiteSpace(NewDescription)) return false;
        return true;
    }

    private async Task AddTransactionAsync(TransactionType transactionType)
    {
        if (!ValidateForAdd(transactionType))
        {
            return; 
        }

        var newTransaction = new BudgetTransaction
        {
            Id = Guid.NewGuid(),
            TransactionType = transactionType,
            Amount = NewAmount,
            Category = NewCategory,
            Description = NewDescription ?? string.Empty,
            Date = NewRecurrenceMonth.HasValue && NewRecurrenceMonth.Value >= 1 && NewRecurrenceMonth.Value <= 12
                ? new DateTime(DateTime.UtcNow.Year, NewRecurrenceMonth.Value, 1)
                : DateTime.UtcNow,
            RecurrenceType = NewRecurrenceType,
            RecurrenceMonth = NewRecurrenceMonth,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(newTransaction);
        await _repo.SaveChangesAsync();
        TransactionsChanged?.Invoke();

        Items.Add(new BudgetTransactionItemViewModel(newTransaction));

        NewAmount = 0m;
        NewCategory = default;
        NewDescription = string.Empty;
        NewRecurrenceType = default;
        NewRecurrenceMonth = null;
    }

    public async Task DeleteSelectedAsync()
    {
        if (SelectedItem?.Transaction is null) return;
        var id = SelectedItem.Transaction.Id;
        await _repo.DeleteAsync(id);
        await _repo.SaveChangesAsync();
        TransactionsChanged?.Invoke();
        Items.Remove(SelectedItem);
        SelectedItem = null;
        UpdateCommandStates();
    }

    public async Task SaveEditAsync()
    {
        if (SelectedItem?.Transaction is null) return;
        var selectedTransaction = SelectedItem.Transaction;
        if (selectedTransaction.Amount <= 0m) return;
        if (selectedTransaction.RecurrenceMonth.HasValue && (selectedTransaction.RecurrenceMonth.Value < 1 || selectedTransaction.RecurrenceMonth.Value > 12))
            return;
        if (selectedTransaction.TransactionType == TransactionType.Expense && string.IsNullOrWhiteSpace(selectedTransaction.Description)) return;

        await _repo.UpdateAsync(selectedTransaction);
        await _repo.SaveChangesAsync();
        TransactionsChanged?.Invoke();
        SelectedItem.IsEditing = false;
        UpdateCommandStates();
    }

    public void CancelEdit()
    {
        if (SelectedItem is null) return;
        SelectedItem.IsEditing = false;
        UpdateCommandStates();
    }

    private void UpdateCommandStates()
    {
        (EditSelectedCommand as DelegateCommand)?.RaiseCanExecuteChanged();
        (SaveEditCommand as DelegateCommand)?.RaiseCanExecuteChanged();
        (CancelEditCommand as DelegateCommand)?.RaiseCanExecuteChanged();
        (DeleteSelectedCommand as DelegateCommand)?.RaiseCanExecuteChanged();
    }
}