namespace MasterYourEnglish.Presentation.ViewModels
{
    using System;
    using System.Windows.Input;
    using MasterYourEnglish.Presentation.ViewModels.Commands;

    public class SidebarViewModel : ViewModelBase
    {
        public SidebarViewModel()
        {
            this.NavigateToProfileCommand = new RelayCommand(
                () => this.NavigationRequested?.Invoke("Profile"));
            this.NavigateToFlashcardsCommand = new RelayCommand(
                () => this.NavigationRequested?.Invoke("Flashcards"));
            this.NavigateToTestsCommand = new RelayCommand(
                () => this.NavigationRequested?.Invoke("Tests"));
            this.NavigateToStatisticsCommand = new RelayCommand(
                () => this.NavigationRequested?.Invoke("Statistics"));
            this.NavigateToSettingsCommand = new RelayCommand(
                () => this.NavigationRequested?.Invoke("Settings"));
        }

        public event Action<string> NavigationRequested;

        public ICommand NavigateToProfileCommand { get; }

        public ICommand NavigateToFlashcardsCommand { get; }

        public ICommand NavigateToTestsCommand { get; }

        public ICommand NavigateToStatisticsCommand { get; }

        public ICommand NavigateToSettingsCommand { get; }
    }
}