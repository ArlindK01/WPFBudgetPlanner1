using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPFBudgetPlanner.VM.Overview;

namespace WPFBudgetPlanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoadedAsync;
        }

        private async void OnLoadedAsync(object sender, RoutedEventArgs e)
        {
            if (DataContext is VM.MainViewModel mainViewModel)
            {
                await mainViewModel.InitializeAsync();
            }

            var saveButtons = FindVisualChildren<Button>(this).Where(button => (button.Content as string) == "Spara");
            foreach (var saveButton in saveButtons)
            {
                saveButton.Click -= OnSettingsSaveClick;
                saveButton.Click += OnSettingsSaveClick;
            }
        }

        private async void OnSettingsSaveClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is VM.MainViewModel mainViewModel && mainViewModel.Budget is BudgetViewModel budgetViewModel)
            {
                await budgetViewModel.SaveSettingsAsync();
            }
        }

        private static System.Collections.Generic.IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent != null)
            {
                for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
                    if (child is T typedChild)
                        yield return typedChild;

                    foreach (T descendant in FindVisualChildren<T>(child))
                        yield return descendant;
                }
            }
        }
    }
}