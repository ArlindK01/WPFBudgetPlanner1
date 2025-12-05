using System;
using WPFBudgetPlanner.Models;

namespace WPFBudgetPlanner.VM.Transactions;

public sealed class BudgetTransactionItemViewModel : WPFBudgetPlanner.VM.ViewModelBase
{
    private BudgetTransaction? _transaction;
    private bool _isSelected;
    private bool _isEditing;
    private string _validationError = string.Empty;

    public BudgetTransactionItemViewModel(BudgetTransaction transaction)
    {
        Transaction = transaction;
    }

    public BudgetTransaction? Transaction
    {
        get => _transaction;
        set
        {
            if (!ReferenceEquals(_transaction, value))
            {
                _transaction = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Amount));
                RaisePropertyChanged(nameof(Category));
                RaisePropertyChanged(nameof(Description));
                RaisePropertyChanged(nameof(Date));
                RaisePropertyChanged(nameof(TransactionType));
                RaisePropertyChanged(nameof(RecurrenceType));
                RaisePropertyChanged(nameof(RecurrenceMonth));
                RaisePropertyChanged(nameof(IsActive));
            }
        }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                RaisePropertyChanged();
            }
        }
    }

    public bool IsEditing
    {
        get => _isEditing;
        set
        {
            if (_isEditing != value)
            {
                _isEditing = value;
                RaisePropertyChanged();
            }
        }
    }

    public string ValidationError
    {
        get => _validationError;
        set
        {
            if (_validationError != value)
            {
                _validationError = value ?? string.Empty;
                RaisePropertyChanged();
            }
        }
    }

    public decimal Amount
    {
        get => Transaction?.Amount ?? 0m;
        set
        {
            if (Transaction is null) return;
            Transaction.Amount = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(Transaction));
        }
    }

    public Category Category
    {
        get => Transaction?.Category ?? default;
        set
        {
            if (Transaction is null) return;
            Transaction.Category = value;
            Transaction.TransactionType = value == Category.Salary ? TransactionType.Income : TransactionType.Expense;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(Transaction));
            RaisePropertyChanged(nameof(TransactionType));
        }
    }

    public string Description
    {
        get => Transaction?.Description ?? string.Empty;
        set
        {
            if (Transaction is null) return;
            Transaction.Description = value ?? string.Empty;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(Transaction));
        }
    }

    public DateTime Date
    {
        get => Transaction?.Date ?? default;
        set
        {
            if (Transaction is null) return;
            Transaction.Date = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(Transaction));
        }
    }

    public TransactionType TransactionType
    {
        get => Transaction?.TransactionType ?? default;
        set
        {
            if (Transaction is null) return;
            Transaction.TransactionType = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(Transaction));
        }
    }

    public RecurrenceType RecurrenceType
    {
        get => Transaction?.RecurrenceType ?? default;
        set
        {
            if (Transaction is null) return;
            Transaction.RecurrenceType = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(Transaction));
        }
    }

    public int? RecurrenceMonth
    {
        get => Transaction?.RecurrenceMonth;
        set
        {
            if (Transaction is null) return;
            Transaction.RecurrenceMonth = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(Transaction));
        }
    }

    public bool IsActive
    {
        get => Transaction?.IsActive ?? false;
        set
        {
            if (Transaction is null) return;
            Transaction.IsActive = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(Transaction));
        }
    }
}