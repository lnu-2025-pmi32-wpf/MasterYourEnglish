using MasterYourEnglish.BLL.Interfaces;
using MasterYourEnglish.Presentation.ViewModels.Commands;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace MasterYourEnglish.Presentation.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly Action _onLoginSuccess;
        private readonly Action _onShowRegister;

        private string _username = "";
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _errorMessage = "";
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand ShowRegisterCommand { get; }
        public ICommand ForgotPasswordCommand { get; } 

        public LoginViewModel(IAuthService authService, Action onLoginSuccess, Action onShowRegister)
        {
            _authService = authService;
            _onLoginSuccess = onLoginSuccess;
            _onShowRegister = onShowRegister;

            LoginCommand = new RelayCommand(async (param) => await OnLogin(param));
            ShowRegisterCommand = new RelayCommand(p => _onShowRegister());
            ForgotPasswordCommand = new RelayCommand(p => { /* TODO: Add logic */ });
        }

        private async Task OnLogin(object parameter)
        {
            if (parameter is PasswordBox passwordBox)
            {
                string password = passwordBox.Password;
                bool success = await _authService.LoginAsync(Username, password);

                if (success)
                {
                    _onLoginSuccess();
                }
                else
                {
                    ErrorMessage = "Invalid username or password.";
                }
            }
        }
    }
}