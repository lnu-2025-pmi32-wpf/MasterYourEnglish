using MasterYourEnglish.BLL.Interfaces;
using MasterYourEnglish.BLL.Services;
using MasterYourEnglish.DAL.Data;
using MasterYourEnglish.DAL.Interfaces;
using MasterYourEnglish.DAL.Repositories;
using MasterYourEnglish.Presentation.ViewModels;
using MasterYourEnglish.Presentation.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Windows;

namespace MasterYourEnglish.Presentation
{
    public partial class App : Application
    {
        private static IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(context.Configuration, services);
                })
                .Build();
        }

        private void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            string connection = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connection));

            // --- Register Repositories ---
            // Add *all* your repositories here
            services.AddScoped<ITestRepository, TestRepository>();
            // services.AddScoped<IFlashcardBundleRepository, FlashcardBundleRepository>();
            // ... etc

            // --- Register BLL Services ---
            // Add *all* your BLL services here
            services.AddTransient<ITestService, TestService>();
            // services.AddTransient<IFlashcardBundleService, FlashcardBundleService>();
            // ... etc

            // --- Register ViewModels ---
            services.AddTransient<MainViewModel>();
            services.AddTransient<SidebarViewModel>();
            services.AddTransient<FlashcardsViewModel>();
            services.AddTransient<TestListViewModel>();
            services.AddTransient<StatisticsViewModel>();
            services.AddTransient<ProfileViewModel>();
            services.AddTransient<SettingsViewModel>();

            // --- Register MainWindow ---
            services.AddSingleton<MainWindow>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();
            base.OnExit(e);
        }
    }
}