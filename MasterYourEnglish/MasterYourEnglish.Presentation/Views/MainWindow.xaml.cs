using MasterYourEnglish.Presentation.ViewModels;
using System.Windows;

namespace MasterYourEnglish.Presentation.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}