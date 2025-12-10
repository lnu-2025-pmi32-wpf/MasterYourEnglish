using System.Windows;
using System.Windows.Controls;

namespace MasterYourEnglish.Presentation.Views
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void ShowCreditsButton_Click(object sender, RoutedEventArgs e)
        {
            var creditsWindow = new CreditsWindow
            {
                Owner = Window.GetWindow(this)
            };
            creditsWindow.ShowDialog();
        }
    }
}