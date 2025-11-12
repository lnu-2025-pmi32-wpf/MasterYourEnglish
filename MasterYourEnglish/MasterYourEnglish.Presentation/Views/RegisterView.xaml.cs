using MasterYourEnglish.Presentation.ViewModels;
using System.Windows;

namespace MasterYourEnglish.Presentation.Views
{
    public partial class RegisterView : Window
    {
        public RegisterView(RegisterViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}