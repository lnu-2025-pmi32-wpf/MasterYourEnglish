namespace MasterYourEnglish.Presentation.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MasterYourEnglish.BLL.DTOs;

    public class MainViewModel : ViewModelBase
    {
        private readonly ProfileViewModel profileVm;
        private readonly FlashcardsViewModel flashcardsVm;
        private readonly TestListViewModel testListVm;
        private readonly StatisticsViewModel statsVm;
        private readonly SettingsViewModel settingsVm;
        private readonly SavedFlashcardsViewModel savedVm;
        private readonly FlashcardSessionViewModel sessionVm;
        private readonly SessionResultsViewModel sessionResultsVm;
        private readonly GenerateBundleViewModel generateBundleVm;
        private readonly CreateBundleViewModel createBundleVm;
        private ViewModelBase currentViewModel;

        public MainViewModel(
            SidebarViewModel sidebarViewModel,
            ProfileViewModel profileVm,
            FlashcardsViewModel flashcardsVm,
            TestListViewModel testListVm,
            StatisticsViewModel statsVm,
            SettingsViewModel settingsVm,
            SavedFlashcardsViewModel savedVm,
            FlashcardSessionViewModel sessionVm,
            SessionResultsViewModel sessionResultsVm,
            GenerateBundleViewModel generateBundleVm,
            CreateBundleViewModel createBundleVm)
        {
            this.SidebarVm = sidebarViewModel;
            this.profileVm = profileVm;
            this.flashcardsVm = flashcardsVm;
            this.testListVm = testListVm;
            this.statsVm = statsVm;
            this.settingsVm = settingsVm;
            this.savedVm = savedVm;
            this.sessionVm = sessionVm;
            this.sessionResultsVm = sessionResultsVm;
            this.generateBundleVm = generateBundleVm;
            this.createBundleVm = createBundleVm;
            this.SidebarVm.NavigationRequested += this.OnNavigationRequested;
            this.flashcardsVm.NavigationRequested += this.OnNavigationRequested;
            this.sessionResultsVm.NavigationRequested += this.OnNavigationRequested;
            this.sessionVm.NavigationRequested += this.OnNavigationRequested;
            this.savedVm.NavigationRequested += this.OnNavigationRequested;
            this.createBundleVm.NavigationRequested += this.OnNavigationRequested;
            this.generateBundleVm.NavigationRequested += this.OnNavigationRequested;
            this.generateBundleVm.SessionGenerated += this.OnSessionGenerated;
            this.CurrentViewModel = this.profileVm;
            this.LoadPageData(this.CurrentViewModel);
        }

        public ViewModelBase CurrentViewModel
        {
            get => this.currentViewModel;
            set => this.SetProperty(ref this.currentViewModel, value);
        }

        public SidebarViewModel SidebarVm { get; }

        private void OnSessionGenerated(List<FlashcardSessionDto> cards)
        {
            this.sessionVm.LoadSession(cards);
            this.CurrentViewModel = this.sessionVm;
        }

        private void OnNavigationRequested(string navigationKey)
        {
            ViewModelBase newPage = this.CurrentViewModel;

            if (navigationKey.StartsWith("FlashcardSession:"))
            {
                var idString = navigationKey.Split(':')[1];
                if (int.TryParse(idString, out int bundleId))
                {
                    this.sessionVm.LoadSession(bundleId);
                    newPage = this.sessionVm;
                }
            }
            else if (navigationKey.StartsWith("SessionResults:"))
            {
                var parts = navigationKey.Split(':');
                int known = int.Parse(parts[1]);
                int total = int.Parse(parts[2]);
                this.sessionResultsVm.ShowResults(known, total);
                newPage = this.sessionResultsVm;
            }
            else
            {
                switch (navigationKey)
                {
                    case "Profile": newPage = this.profileVm; break;
                    case "Flashcards": newPage = this.flashcardsVm; break;
                    case "Tests": newPage = this.testListVm; break;
                    case "Statistics": newPage = this.statsVm; break;
                    case "Settings": newPage = this.settingsVm; break;
                    case "SavedFlashcards": newPage = this.savedVm; break;
                    case "TestSavedFlashcards":
                        this.sessionVm.LoadSessionFromSaved();
                        newPage = this.sessionVm;
                        break;
                    case "GenerateBundle":
                        newPage = this.generateBundleVm;
                        break;
                    case "CreateBundle":
                        newPage = this.createBundleVm;
                        break;
                }
            }

            if (newPage != this.CurrentViewModel)
            {
                this.CurrentViewModel = newPage;
                this.LoadPageData(this.CurrentViewModel);
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
