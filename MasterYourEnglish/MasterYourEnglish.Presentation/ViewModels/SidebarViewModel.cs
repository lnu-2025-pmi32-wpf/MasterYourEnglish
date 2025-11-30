namespace MasterYourEnglish.Presentation.ViewModels
{
    using System;
    using System.Windows.Input;
    using MasterYourEnglish.Presentation.ViewModels.Commands;
    using Microsoft.Extensions.Logging;

    public class SidebarViewModel : ViewModelBase
    {
        private readonly ILogger<SidebarViewModel> logger;

        public SidebarViewModel(ILogger<SidebarViewModel> logger)
        {
            this.logger = logger;

            this.NavigateToProfileCommand = new RelayCommand(
                p =>
                {
                    this.logger.LogInformation("Sidebar navigation requested: Profile");
                    this.NavigationRequested?.Invoke("Profile");
                });
            this.NavigateToFlashcardsCommand = new RelayCommand(
                p =>
                {
                    this.logger.LogInformation("Sidebar navigation requested: Flashcards");
                    this.NavigationRequested?.Invoke("Flashcards");
                });
            this.NavigateToTestsCommand = new RelayCommand(
                p =>
                {
                    this.logger.LogInformation("Sidebar navigation requested: Tests");
                    this.NavigationRequested?.Invoke("Tests");
                });
            this.NavigateToStatisticsCommand = new RelayCommand(
                p =>
                {
                    this.logger.LogInformation("Sidebar navigation requested: Statistics");
                    this.NavigationRequested?.Invoke("Statistics");
                });
            this.NavigateToSettingsCommand = new RelayCommand(
                p =>
                {
                    this.logger.LogInformation("Sidebar navigation requested: Settings");
                    this.NavigationRequested?.Invoke("Settings");
                });

            this.logger.LogInformation("SidebarViewModel initialized.");
        }

        public event Action<string> NavigationRequested;

        public ICommand NavigateToProfileCommand { get; }

        public ICommand NavigateToFlashcardsCommand { get; }

        public ICommand NavigateToTestsCommand { get; }

        public ICommand NavigateToStatisticsCommand { get; }

        public ICommand NavigateToSettingsCommand { get; }
    }
}