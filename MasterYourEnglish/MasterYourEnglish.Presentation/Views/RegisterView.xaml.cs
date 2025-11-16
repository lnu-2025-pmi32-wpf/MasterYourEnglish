namespace MasterYourEnglish.Presentation.Views
{
    using System.Windows;
    using MasterYourEnglish.Presentation.ViewModels;

    public partial class RegisterView : Window
    {
        public RegisterView(RegisterViewModel viewModel)
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}