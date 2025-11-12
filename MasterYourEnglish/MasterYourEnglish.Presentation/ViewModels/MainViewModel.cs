using System;
using System.Threading.Tasks;

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
        private readonly SessionResultsViewModel _sessionResultsVm;

        public MainViewModel(
            SidebarViewModel sidebarViewModel,
            ProfileViewModel profileVm,
            FlashcardsViewModel flashcardsVm,
            TestListViewModel testListVm,
            StatisticsViewModel statsVm,
            SettingsViewModel settingsVm,
            SavedFlashcardsViewModel savedVm,
            FlashcardSessionViewModel sessionVm,
            SessionResultsViewModel sessionResultsVm)
        {
            SidebarVm = sidebarViewModel;
            _profileVm = profileVm;
            _flashcardsVm = flashcardsVm;
            _testListVm = testListVm;
            _statsVm = statsVm;
            _settingsVm = settingsVm;
            _savedVm = savedVm;
            _sessionVm = sessionVm;
            _sessionResultsVm = sessionResultsVm;

            SidebarVm.NavigationRequested += OnNavigationRequested;
            _flashcardsVm.NavigationRequested += OnNavigationRequested;

            _sessionResultsVm.NavigationRequested += OnNavigationRequested;
            _sessionVm.NavigationRequested += OnNavigationRequested;

            CurrentViewModel = _profileVm;
            LoadPageData(CurrentViewModel);
        }

        private void OnNavigationRequested(string navigationKey)
        {
            ViewModelBase newPage = CurrentViewModel;

            if (navigationKey.StartsWith("FlashcardSession:"))
            {
                var idString = navigationKey.Split(':')[1];
                if (int.TryParse(idString, out int bundleId))
                {
                    _sessionVm.LoadSession(bundleId);
                    newPage = _sessionVm;
                }
            }

            else if (navigationKey.StartsWith("SessionResults:"))
            {
                var parts = navigationKey.Split(':');
                int known = int.Parse(parts[1]);
                int total = int.Parse(parts[2]);

                _sessionResultsVm.ShowResults(known, total);
                newPage = _sessionResultsVm;
            }
            else
            {
                switch (navigationKey)
                {
                    case "Profile": newPage = _profileVm; break;
                    case "Flashcards": newPage = _flashcardsVm; break;
                    case "Tests": newPage = _testListVm; break;
                    case "Statistics": newPage = _statsVm; break;
                    case "Settings": newPage = _settingsVm; break;
                    case "SavedFlashcards": newPage = _savedVm; break;
                }
            }

            if (newPage != CurrentViewModel)
            {
                CurrentViewModel = newPage;

                LoadPageData(CurrentViewModel);
            }
        }

        private async void LoadPageData(ViewModelBase vm)
        {
            if (vm is IPageViewModel page)
            {
                await page.LoadDataAsync();
            }
        }
    }
}