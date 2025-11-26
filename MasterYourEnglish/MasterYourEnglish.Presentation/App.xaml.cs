namespace MasterYourEnglish.Presentation
{
    using System.IO;
    using System.Windows;
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

    public partial class App : Application
    {
        private static IHost host;
        private Window currentWindow;

        public App()
        {
            host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    this.ConfigureServices(context.Configuration, services);
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await host.StartAsync();
            this.ShowLoginView();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await host.StopAsync();
            host.Dispose();
            base.OnExit(e);
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
            services.AddScoped<IRepository<Question>, Repository<Question>>();
            services.AddScoped<IRepository<QuestionOption>, Repository<QuestionOption>>();
            services.AddScoped<IRepository<TestQuestion>, Repository<TestQuestion>>();
            services.AddScoped<ITestAttemptRepository, TestAttemptRepository>();
            services.AddScoped<IRepository<TestAttemptAnswer>, Repository<TestAttemptAnswer>>();
            services.AddScoped<IRepository<TestAttemptAnswerSelectedOption>, Repository<TestAttemptAnswerSelectedOption>>();

            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<ITestService, TestService>();
            services.AddTransient<IUserService, UserService>();
            services.AddSingleton<ICurrentUserService, CurrentUserService>();
            services.AddTransient<IFlashcardBundleService, FlashcardBundleService>();
            services.AddTransient<IFlashcardService, FlashcardService>();
            services.AddTransient<ITopicService, TopicService>();
            services.AddTransient<IStatisticsService, StatisticsService>();


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
            services.AddTransient<TestSessionViewModel>();
            services.AddTransient<CreateTestViewModel>();
            services.AddTransient<GenerateTestViewModel>();
        }

        private void ShowLoginView()
        {
            var loginViewModel = new LoginViewModel(
                host.Services.GetRequiredService<IAuthService>(),
                onLoginSuccess: () =>
                {
                    this.ShowMainWindow();
                },
                onShowRegister: () =>
                {
                    this.ShowRegisterView();
                });

            var newWindow = new LoginView(loginViewModel);

            newWindow.Show();

            this.currentWindow?.Close();

            this.currentWindow = newWindow;
        }

        private void ShowRegisterView()
        {
            var registerViewModel = new RegisterViewModel(
                host.Services.GetRequiredService<IAuthService>(),
                onRegisterSuccess: () =>
                {
                    this.ShowLoginView();
                },
                onShowLogin: () =>
                {
                    this.ShowLoginView();
                });

            var newWindow = new RegisterView(registerViewModel);

            newWindow.Show();

            this.currentWindow?.Close();

            this.currentWindow = newWindow;
        }

        private void ShowMainWindow()
        {
            var newWindow = host.Services.GetRequiredService<MainWindow>();

            newWindow.Show();

            this.currentWindow?.Close();

            this.currentWindow = newWindow;
        }
    }
}