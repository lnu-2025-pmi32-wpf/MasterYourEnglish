using System;

namespace MasterYourEnglish.Presentation.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public SidebarViewModel SidebarVm { get; }

        private readonly ProfileViewModel _profileVm;
        private readonly FlashcardsViewModel _flashcardsVm;
        private readonly TestListViewModel _testListVm;
        private readonly StatisticsViewModel _statsVm;
        private readonly SettingsViewModel _settingsVm;
        private readonly SavedFlashcardsViewModel _savedVm;
        private readonly FlashcardSessionViewModel _sessionVm;

        public MainViewModel(
            SidebarViewModel sidebarViewModel,
            ProfileViewModel profileVm,
            FlashcardsViewModel flashcardsVm,
            TestListViewModel testListVm,
            StatisticsViewModel statsVm,
            SettingsViewModel settingsVm,
            SavedFlashcardsViewModel savedVm,
            FlashcardSessionViewModel sessionVm)
        {
            SidebarVm = sidebarViewModel;

            _profileVm = profileVm;
            _flashcardsVm = flashcardsVm;
            _testListVm = testListVm;
            _statsVm = statsVm;
            _settingsVm = settingsVm;
            _savedVm = savedVm;
            _sessionVm = sessionVm;

            SidebarVm.NavigationRequested += OnNavigationRequested;
            _flashcardsVm.NavigationRequested += OnNavigationRequested;
            CurrentViewModel = _profileVm;
        }

        private void OnNavigationRequested(string navigationKey)
        {
            if (navigationKey.StartsWith("FlashcardSession:"))
            {
                var idString = navigationKey.Split(':')[1];
                if (int.TryParse(idString, out int bundleId))
                {
                    _sessionVm.LoadSession(bundleId);
                    CurrentViewModel = _sessionVm;
                }
            }
            else
            {
                switch (navigationKey)
                {
                    case "Profile":
                        CurrentViewModel = _profileVm;
                        break;
                    case "Flashcards":
                        CurrentViewModel = _flashcardsVm;
                        break;
                    case "Tests":
                        CurrentViewModel = _testListVm;
                        break;
                    case "Statistics":
                        CurrentViewModel = _statsVm;
                        break;
                    case "Settings":
                        CurrentViewModel = _settingsVm;
                        break;
                    case "SavedFlashcards":
                        CurrentViewModel = _savedVm;
                        break;
                }
            }
        }
    }
}