namespace MasterYourEnglish.Presentation.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Input;
    using MasterYourEnglish.BLL.Interfaces;
    using MasterYourEnglish.BLL.Models.DTOs;
    using MasterYourEnglish.Presentation.ViewModels.Commands;

    public class RegisterViewModel : ViewModelBase
    {
        private readonly IAuthService authService;
        private readonly Action onRegisterSuccess;
        private readonly Action onShowLogin;
        private string firstName = string.Empty;
        private string lastName = string.Empty;
        private string username = string.Empty;
        private string email = string.Empty;
        private string errorMessage = string.Empty;

        public RegisterViewModel(IAuthService authService, Action onRegisterSuccess, Action onShowLogin)
        {
            this.authService = authService;
            this.onRegisterSuccess = onRegisterSuccess;
            this.onShowLogin = onShowLogin;
            this.RegisterCommand = new RelayCommand(async (param) => await this.OnRegister(param));
            this.ShowLoginCommand = new RelayCommand(p => this.onShowLogin());
        }

        public string FirstName
        {
            get => this.firstName;
            set => this.SetProperty(ref this.firstName, value);
        }

        public string LastName
        {
            get => this.lastName;
            set => this.SetProperty(ref this.lastName, value);
        }

        public string Username
        {
            get => this.username;
            set => this.SetProperty(ref this.username, value);
        }

        public string Email
        {
            get => this.email;
            set => this.SetProperty(ref this.email, value);
        }

        public string ErrorMessage
        {
            get => this.errorMessage;
            set => this.SetProperty(ref this.errorMessage, value);
        }

        public ICommand RegisterCommand { get; }

        public ICommand ShowLoginCommand { get; }

        private async Task OnRegister(object parameter)
        {
            if (parameter is not PasswordBox passwordBox)
            {
                return;
            }

            string password = passwordBox.Password;
            if (string.IsNullOrWhiteSpace(this.FirstName) || string.IsNullOrWhiteSpace(this.Username) || string.IsNullOrWhiteSpace(password))
            {
                this.ErrorMessage = "Please fill out all required fields.";
                return;
            }

            var registerDto = new RegisterDto
            {
                FirstName = this.FirstName,
                LastName = this.LastName,
                Username = this.Username,
                Email = this.Email,
                Password = password,
            };

            bool success = await this.authService.RegisterAsync(registerDto);
            if (success)
            {
                this.onRegisterSuccess();
            }
            else
            {
                this.ErrorMessage = "Registration failed. Username may be taken.";
            }
        }
    }
}
