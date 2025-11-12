using MasterYourEnglish.Presentation.ViewModels;
using System.Windows;

namespace MasterYourEnglish.Presentation.Views
{
    public partial class LoginView : Window
    {
        public LoginView(LoginViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}