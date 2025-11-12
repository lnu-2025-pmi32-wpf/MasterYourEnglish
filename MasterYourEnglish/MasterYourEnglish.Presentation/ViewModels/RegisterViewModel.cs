using MasterYourEnglish.BLL.Interfaces;
using MasterYourEnglish.BLL.Models.DTOs;
using MasterYourEnglish.Presentation.ViewModels.Commands;
using System;
using System.Threading.Tasks;
using System.Windows.Controls; 
using System.Windows.Input;

namespace MasterYourEnglish.Presentation.ViewModels
{
    public class RegisterViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly Action _onRegisterSuccess;
        private readonly Action _onShowLogin;

        private string _firstName = "";
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        private string _lastName = "";
        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        private string _username = "";
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _email = "";
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _errorMessage = "";
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand RegisterCommand { get; }
        public ICommand ShowLoginCommand { get; }

        public RegisterViewModel(IAuthService authService, Action onRegisterSuccess, Action onShowLogin)
        {
            _authService = authService;
            _onRegisterSuccess = onRegisterSuccess;
            _onShowLogin = onShowLogin;

            RegisterCommand = new RelayCommand(async (param) => await OnRegister(param));
            ShowLoginCommand = new RelayCommand(p => _onShowLogin());
        }

        private async Task OnRegister(object parameter)
        {
            if (parameter is not PasswordBox passwordBox) return;

            string password = passwordBox.Password;

            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = "Please fill out all required fields.";
                return;
            }

            var registerDto = new RegisterDto
            {
                FirstName = this.FirstName,
                LastName = this.LastName,
                Username = this.Username,
                Email = this.Email,
                Password = password
            };

            bool success = await _authService.RegisterAsync(registerDto);

            if (success)
            {
                _onRegisterSuccess();
            }
            else
            {
                ErrorMessage = "Registration failed. Username may be taken.";
            }
        }
    }
}