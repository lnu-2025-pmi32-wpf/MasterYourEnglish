namespace MasterYourEnglish.Presentation.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using System.Windows; // Додано для доступу до потоку UI (Dispatcher)
    using System.Windows.Controls;
    using System.Windows.Input;
    using MasterYourEnglish.BLL.Interfaces;
    using MasterYourEnglish.Presentation.ViewModels.Commands;
    using Microsoft.Extensions.Logging;

    public class LoginViewModel : ViewModelBase
    {
        private readonly IAuthService authService;
        private readonly ILogger<LoginViewModel> logger;
        private readonly Action onLoginSuccess;
        private readonly Action onShowRegister;
        private string username = string.Empty;
        private string errorMessage = string.Empty;

        public LoginViewModel(
            IAuthService authService,
            ILogger<LoginViewModel> logger,
            Action onLoginSuccess,
            Action onShowRegister)
        {
            this.authService = authService;
            this.logger = logger;
            this.onLoginSuccess = onLoginSuccess;
            this.onShowRegister = onShowRegister;

            this.LoginCommand = new RelayCommand(async (param) => await this.OnLogin(param), this.CanLogin);
            this.ShowRegisterCommand = new RelayCommand(p => this.onShowRegister());
            this.ForgotPasswordCommand = new RelayCommand(p =>
            {
                this.logger.LogInformation("Forgot Password functionality invoked.");
            });

            this.logger.LogInformation("LoginViewModel initialized.");
        }

        public string Username
        {
            get => this.username;
            set => this.SetProperty(ref this.username, value);
        }

        public string ErrorMessage
        {
            get => this.errorMessage;
            set => this.SetProperty(ref this.errorMessage, value);
        }

        public ICommand LoginCommand { get; }

        public ICommand ShowRegisterCommand { get; }

        public ICommand ForgotPasswordCommand { get; }

        private bool CanLogin(object parameter)
        {
            if (parameter is PasswordBox passwordBox)
            {
                return !string.IsNullOrWhiteSpace(this.Username) && !string.IsNullOrWhiteSpace(passwordBox.Password);
            }

            return !string.IsNullOrWhiteSpace(this.Username);
        }

        private async Task OnLogin(object parameter)
        {
            if (parameter is not PasswordBox passwordBox)
            {
                this.logger.LogWarning("Login failed: Password parameter (PasswordBox) is missing.");
                this.ErrorMessage = "System error: Password field not found.";
                return;
            }

            string password = passwordBox.Password;
            if (string.IsNullOrWhiteSpace(this.Username) || string.IsNullOrWhiteSpace(password))
            {
                this.ErrorMessage = "Please enter both username and password.";
                this.logger.LogWarning("Login failed: Missing required credentials.");
                return;
            }

            this.logger.LogInformation("Attempting login for user: {Username}", this.Username);
            try
            {
                bool success = await this.authService.LoginAsync(this.Username, password);

                if (success)
                {
                    this.ErrorMessage = string.Empty;
                    this.logger.LogInformation("User {Username} logged in successfully.", this.Username);

                    Application.Current.Dispatcher.Invoke(this.onLoginSuccess);
                }
                else
                {
                    this.ErrorMessage = "Invalid username or password.";
                    this.logger.LogWarning("Failed login attempt for user {Username}. Invalid credentials.", this.Username);
                }
            }
            catch (Exception ex)
            {
                this.ErrorMessage = "An unexpected error occurred during login.";
                this.logger.LogError(ex, "An exception occurred during login for user {Username}.", this.Username);
            }
        }
    }
}