namespace MasterYourEnglish.Presentation.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Input;
    using MasterYourEnglish.BLL.Interfaces;
    using MasterYourEnglish.Presentation.ViewModels.Commands;

    public class LoginViewModel : ViewModelBase
    {
        private readonly IAuthService authService;
        private readonly Action onLoginSuccess;
        private readonly Action onShowRegister;
        private string username = string.Empty;
        private string errorMessage = string.Empty;

        public LoginViewModel(IAuthService authService, Action onLoginSuccess, Action onShowRegister)
        {
            this.authService = authService;
            this.onLoginSuccess = onLoginSuccess;
            this.onShowRegister = onShowRegister;
            this.LoginCommand = new RelayCommand(async (param) => await this.OnLogin(param));
            this.ShowRegisterCommand = new RelayCommand(p => this.onShowRegister());
            this.ForgotPasswordCommand = new RelayCommand(p => { /* TODO: Add logic */ });
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

        private async Task OnLogin(object parameter)
        {
            if (parameter is PasswordBox passwordBox)
            {
                string password = passwordBox.Password;
                bool success = await this.authService.LoginAsync(this.Username, password);
                if (success)
                {
                    this.onLoginSuccess();
                }
                else
                {
                    this.ErrorMessage = "Invalid username or password.";
                }
            }
        }
    }
}