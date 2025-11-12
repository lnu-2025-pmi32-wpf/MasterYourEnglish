using MasterYourEnglish.BLL.Interfaces;
using MasterYourEnglish.BLL.Services;
using MasterYourEnglish.DAL.Data;
using MasterYourEnglish.DAL.Entities;
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
        private Window _currentWindow;

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

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITestRepository, TestRepository>();
            services.AddScoped<IFlashcardBundleRepository, FlashcardBundleRepository>();
            services.AddScoped<IFlashcardRepository, FlashcardRepository>();
            services.AddScoped<IFlashcardsBundleAttemptRepository, FlashcardsBundleAttemptRepository>();
            services.AddScoped<IRepository<Topic>, Repository<Topic>>();

            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<ITestService, TestService>();
            services.AddTransient<IUserService, UserService>();
            services.AddSingleton<ICurrentUserService, CurrentUserService>();
            services.AddTransient<IFlashcardBundleService, FlashcardBundleService>();
            services.AddTransient<IFlashcardService, FlashcardService>();
            services.AddTransient<ITopicService, TopicService>();

            services.AddScoped<IRepository<FlashcardAttemptAnswer>, Repository<FlashcardAttemptAnswer>>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<SidebarViewModel>();
            services.AddTransient<FlashcardsViewModel>();
            services.AddTransient<TestListViewModel>();
            services.AddTransient<StatisticsViewModel>();
            services.AddTransient<ProfileViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SavedFlashcardsViewModel>();
            services.AddTransient<FlashcardSessionViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddSingleton<MainWindow>();
            services.AddTransient<LoginView>();
            services.AddTransient<RegisterView>();
            services.AddTransient<SessionResultsViewModel>();
            services.AddTransient<GenerateBundleViewModel>();
            services.AddTransient<CreateBundleViewModel>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();
            ShowLoginView();
        }

        private void ShowLoginView()
        {
            var loginViewModel = new LoginViewModel(
                _host.Services.GetRequiredService<IAuthService>(),
                onLoginSuccess: () => {
                    ShowMainWindow();
                },
                onShowRegister: () => {
                    ShowRegisterView();
                }
            );

            var newWindow = new LoginView(loginViewModel);

            newWindow.Show();

            _currentWindow?.Close();

            _currentWindow = newWindow;
        }
        private void ShowRegisterView()
        {
            var registerViewModel = new RegisterViewModel(
                _host.Services.GetRequiredService<IAuthService>(),
                onRegisterSuccess: () => {
                    ShowLoginView();
                },
                onShowLogin: () => {
                    ShowLoginView();
                }
            );

            var newWindow = new RegisterView(registerViewModel);

            newWindow.Show();

            _currentWindow?.Close();

            _currentWindow = newWindow;
        }

        private void ShowMainWindow()
        {
            var newWindow = _host.Services.GetRequiredService<MainWindow>();

            newWindow.Show();

            _currentWindow?.Close();

            _currentWindow = newWindow;
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();
            base.OnExit(e);
        }
    }
}