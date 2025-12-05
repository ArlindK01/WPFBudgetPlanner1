using System.Windows;
using System.Windows.Controls;

namespace WPFBudgetPlanner.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
    }

    private async void OnSaveSettingsClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is VM.Overview.BudgetViewModel vm)
        {
            await vm.SaveSettingsAsync();
        }
    }
}