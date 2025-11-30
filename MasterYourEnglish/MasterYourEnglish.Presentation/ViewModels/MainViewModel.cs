namespace MasterYourEnglish.Presentation.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MasterYourEnglish.BLL.DTOs;
    using Microsoft.Extensions.Logging;

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
        private readonly CreateTestViewModel createTestVm;
        private readonly GenerateTestViewModel generateTestVm;
        private readonly TestSessionViewModel testSessionVm;
        private readonly ILogger<MainViewModel> logger;
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
            CreateBundleViewModel createBundleVm,
            CreateTestViewModel createTestVm,
            GenerateTestViewModel generateTestVm,
            TestSessionViewModel testSessionVm,
            ILogger<MainViewModel> logger)
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
            this.createTestVm = createTestVm;

            this.generateTestVm = generateTestVm;
            this.testSessionVm = testSessionVm;
            this.logger = logger;

            this.SidebarVm.NavigationRequested += this.OnNavigationRequested;
            this.flashcardsVm.NavigationRequested += this.OnNavigationRequested;
            this.sessionResultsVm.NavigationRequested += this.OnNavigationRequested;
            this.sessionVm.NavigationRequested += this.OnNavigationRequested;
            this.savedVm.NavigationRequested += this.OnNavigationRequested;
            this.createBundleVm.NavigationRequested += this.OnNavigationRequested;
            this.generateBundleVm.NavigationRequested += this.OnNavigationRequested;
            this.generateBundleVm.SessionGenerated += this.OnSessionGenerated;
            this.createTestVm.NavigationRequested += this.OnNavigationRequested;

            this.generateTestVm.NavigationRequested += this.OnNavigationRequested;
            this.generateTestVm.TestSessionGenerated += this.OnTestSessionGenerated;
            this.testListVm.NavigationRequested += this.OnNavigationRequested;
            this.testSessionVm.NavigationRequested += this.OnNavigationRequested;
            this.CurrentViewModel = this.profileVm;
            this.LoadPageData(this.CurrentViewModel);
            this.logger.LogInformation("Main application shell initialized.");
        }

        public ViewModelBase CurrentViewModel
        {
            get => this.currentViewModel;
            set => this.SetProperty(ref this.currentViewModel, value);
        }

        public SidebarViewModel SidebarVm { get; }

        private void OnSessionGenerated(List<FlashcardSessionDto> cards)
        {
            this.logger.LogInformation("Generated flashcard session received. Starting session.");
            this.sessionVm.LoadSession(cards);
            this.CurrentViewModel = this.sessionVm;
        }

        private void OnTestSessionGenerated(List<TestSessionDto> tests)
        {
            if (tests != null && tests.Count > 0)
            {
                this.logger.LogInformation("Generated test session received. Starting test ID: {TestId}", tests[0].TestId);
                this.testSessionVm.LoadTest(tests[0]);
                this.CurrentViewModel = this.testSessionVm;
            }
            else
            {
                this.logger.LogWarning("Generated test session received was null or empty.");
            }
        }

        private void OnNavigationRequested(string navigationKey)
        {
            ViewModelBase newPage = this.CurrentViewModel;
            this.logger.LogInformation("Navigation requested: {Key}", navigationKey);

            if (navigationKey.StartsWith("FlashcardSession:"))
            {
                var idString = navigationKey.Split(':')[1];
                if (int.TryParse(idString, out int bundleId))
                {
                    this.sessionVm.LoadSession(bundleId);
                    newPage = this.sessionVm;
                }
                else
                {
                    this.logger.LogError("Failed to parse bundle ID from navigation key: {Key}", navigationKey);
                }
            }
            else if (navigationKey.StartsWith("SessionResults:"))
            {
                var parts = navigationKey.Split(':');
                if (parts.Length < 3 || !int.TryParse(parts[1], out int known) || !int.TryParse(parts[2], out int total))
                {
                    this.logger.LogError("Invalid SessionResults format: {Key}", navigationKey);
                    return;
                }

                string returnKey = parts.Length > 3 ? parts[3] : "Flashcards";

                this.logger.LogDebug("Showing results: Known={Known}, Total={Total}, ReturnKey={ReturnKey}", known, total, returnKey);

                this.sessionResultsVm.ShowResults(known, total, returnKey);

                newPage = this.sessionResultsVm;
            }
            else if (navigationKey.StartsWith("TestSession:"))
            {
                var idString = navigationKey.Split(':')[1];
                if (int.TryParse(idString, out int testId))
                {
                    this.testSessionVm.LoadTest(testId);
                    newPage = this.testSessionVm;
                }
                else
                {
                    this.logger.LogError("Failed to parse test ID from navigation key: {Key}", navigationKey);
                }
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
                        this.logger.LogInformation("Navigating to saved flashcards session.");
                        this.sessionVm.LoadSessionFromSaved();
                        newPage = this.sessionVm;
                        break;
                    case "GenerateBundle": newPage = this.generateBundleVm; break;
                    case "CreateBundle": newPage = this.createBundleVm; break;
                    case "CreateTest": newPage = this.createTestVm; break;
                    case "GenerateTest": newPage = this.generateTestVm; break;
                    default:
                        this.logger.LogWarning("Unrecognized navigation key: {Key}", navigationKey);
                        return;
                }
            }

            if (newPage != this.CurrentViewModel)
            {
                this.logger.LogInformation(
                    "Switching view model from {Old} to {New}",
                    this.CurrentViewModel.GetType().Name,
                    newPage.GetType().Name);
                this.CurrentViewModel = newPage;
                this.LoadPageData(this.CurrentViewModel);
            }
        }

        private async void LoadPageData(ViewModelBase vm)
        {
            if (vm is IPageViewModel page)
            {
                this.logger.LogDebug("Calling LoadDataAsync for {ViewModelName}", vm.GetType().Name);
                await page.LoadDataAsync();
            }
        }
    }
}