namespace MasterYourEnglish.Presentation.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using MasterYourEnglish.BLL.DTOs;
    using MasterYourEnglish.BLL.Interfaces;
    using MasterYourEnglish.BLL.Models.DTOs;
    using MasterYourEnglish.Presentation.ViewModels.Commands;
    using Microsoft.Extensions.Logging;

    public class TestListViewModel : ViewModelBase, IPageViewModel
    {
        private readonly ITestService testService;
        private readonly ILogger<TestListViewModel> logger;
        private bool isLoading = false;

        public ObservableCollection<TestCardDto> TestCards { get; }

        public event Action<string> NavigationRequested;

        private string searchTerm;

        public string SearchTerm
        {
            get => this.searchTerm;
            set
            {
                if (this.SetProperty(ref this.searchTerm, value))
                {
                    _ = this.LoadDataAsync();
                }
            }
        }

        public List<string> SortOptions { get; }

        private string selectedSortOption;

        public string SelectedSortOption
        {
            get => this.selectedSortOption;
            set
            {
                if (this.SetProperty(ref this.selectedSortOption, value))
                {
                    _ = this.LoadDataAsync();
                }
            }
        }

        public ICommand NavigateToCreateCommand { get; }

        public ICommand NavigateToGenerateCommand { get; }

        public ICommand StartTestCommand { get; }

        public TestListViewModel(
            ITestService testService,
            ILogger<TestListViewModel> logger)
        {
            this.testService = testService;
            this.logger = logger;

            this.TestCards = new ObservableCollection<TestCardDto>();

            this.SortOptions = new List<string> { "Name (A-Z)", "Name (Z-A)", "Level (Easy-Hard)", "Level (Hard-Easy)" };
            this.selectedSortOption = this.SortOptions.First();

            this.NavigateToCreateCommand = new RelayCommand(p => this.NavigationRequested?.Invoke("CreateTest"));
            this.NavigateToGenerateCommand = new RelayCommand(p => this.NavigationRequested?.Invoke("GenerateTest"));
            this.StartTestCommand = new RelayCommand(this.OnStartTest);

            this.logger.LogInformation("TestListViewModel initialized.");
        }

        public async Task LoadDataAsync()
        {
            if (this.isLoading)
            {
                this.logger.LogDebug("LoadDataAsync skipped, already loading.");
                return;
            }

            this.isLoading = true;
            this.logger.LogInformation(
                "Starting to load published tests. SearchTerm: '{Term}', Sort: {Sort}",
                this.searchTerm,
                this.selectedSortOption);

            try
            {
                string sortBy = this.selectedSortOption.Contains("Name") ? "Name" : "Level";
                bool ascending = this.selectedSortOption.Contains("(A-Z)") || this.selectedSortOption.Contains("(Easy-Hard)");

                var tests = await this.testService.GetPublishedTestsAsync(this.searchTerm, sortBy, ascending);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.TestCards.Clear();
                    foreach (var test in tests)
                    {
                        this.TestCards.Add(test);
                    }
                });
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to load published tests.");
            }
            finally
            {
                this.isLoading = false;
            }
        }

        private void OnStartTest(object parameter)
        {
            if (parameter is TestCardDto test)
            {
                this.logger.LogInformation("Starting test session for test ID: {Id}", test.TestId);
                this.NavigationRequested?.Invoke($"TestSession:{test.TestId}");
            }
            else
            {
                this.logger.LogWarning("Attempted to start test session without valid test parameter.");
            }
        }
    }
}