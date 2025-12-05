using System.Globalization;
using System.Threading;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WPFBudgetPlanner.Data;
using WPFBudgetPlanner.Services;
using WPFBudgetPlanner.VM;
using WPFBudgetPlanner.VM.Overview;
using WPFBudgetPlanner.VM.Transactions;

namespace WPFBudgetPlanner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public ServiceProvider Services { get; private set; } = null!;
        private IServiceScope? _scope;

        public App()
        {
            var services = new ServiceCollection();

            services.AddDbContextFactory<BudgetDbContext>();

            services.AddScoped<IBudgetTransactionRepository, BudgetTransactionRepository>();
            services.AddScoped<IUserSettingRepository, UserSettingRepository>();

            
            services.AddScoped<ISalaryCalculator, SalaryCalculator>();
            services.AddScoped<IBudgetForecastService, BudgetForecastService>();

            services.AddTransient<BudgetTransactionListViewModel>();
            services.AddTransient<BudgetViewModel>();
            services.AddTransient<MainViewModel>();

            Services = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _scope = Services.CreateScope();
            var vm = _scope.ServiceProvider.GetRequiredService<MainViewModel>();

            var mainWindow = new MainWindow();
            mainWindow.DataContext = vm;
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _scope?.Dispose();
            Services.Dispose();
            base.OnExit(e);
        }
    }
}
