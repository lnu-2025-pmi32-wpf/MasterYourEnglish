using MasterYourEnglish.Presentation.ViewModels.Commands;
using System;
using System.Windows.Input;

namespace MasterYourEnglish.Presentation.ViewModels
{
    public class SidebarViewModel : ViewModelBase
    {
        public event Action<string> NavigationRequested;

        public ICommand NavigateToProfileCommand { get; }
        public ICommand NavigateToFlashcardsCommand { get; }
        public ICommand NavigateToTestsCommand { get; }
        public ICommand NavigateToStatisticsCommand { get; }
        public ICommand NavigateToSettingsCommand { get; }

        public SidebarViewModel()
        {
            NavigateToProfileCommand = new RelayCommand(
                () => NavigationRequested?.Invoke("Profile")
            );
            NavigateToFlashcardsCommand = new RelayCommand(
                () => NavigationRequested?.Invoke("Flashcards")
            );
            NavigateToTestsCommand = new RelayCommand(
                () => NavigationRequested?.Invoke("Tests")
            );
            NavigateToStatisticsCommand = new RelayCommand(
                () => NavigationRequested?.Invoke("Statistics")
            );
            NavigateToSettingsCommand = new RelayCommand(
                () => NavigationRequested?.Invoke("Settings")
            );
        }
    }
}