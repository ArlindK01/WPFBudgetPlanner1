using System;
using System.Windows;
using System.Windows.Controls;

namespace WPFBudgetPlanner.Views;

public partial class BudgetView : UserControl
{
    public BudgetView()
    {
        InitializeComponent();
    }

    private async void OnRefreshClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is VM.Overview.BudgetViewModel vm)
        {
            var year = vm.SelectedMonth.Year;
            var month = vm.SelectedMonth.Month;
            await vm.RefreshAsync(year, month);
        }
    }

    private async void OnSaveSettingsClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is VM.Overview.BudgetViewModel vm)
        {
            await vm.SaveSettingsAsync();
        }
    }
}