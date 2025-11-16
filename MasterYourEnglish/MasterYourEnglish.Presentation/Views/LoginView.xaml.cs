namespace MasterYourEnglish.Presentation.Views
{
    using System.Windows;
    using MasterYourEnglish.Presentation.ViewModels;

    public partial class LoginView : Window
    {
        public LoginView(LoginViewModel viewModel)
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}