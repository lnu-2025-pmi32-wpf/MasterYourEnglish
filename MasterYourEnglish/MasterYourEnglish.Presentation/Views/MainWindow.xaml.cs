namespace MasterYourEnglish.Presentation.Views
{
    using System.Windows;
    using MasterYourEnglish.Presentation.ViewModels;

    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}