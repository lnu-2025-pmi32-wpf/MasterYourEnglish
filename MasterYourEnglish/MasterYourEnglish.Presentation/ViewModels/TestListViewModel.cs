namespace MasterYourEnglish.Presentation.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using MasterYourEnglish.BLL.Interfaces;
    using MasterYourEnglish.BLL.Models.DTOs;
    using MasterYourEnglish.Presentation.ViewModels.Commands;

    public class TestListViewModel : ViewModelBase, IPageViewModel
    {
        private readonly ITestService testService;
        private bool isLoading = false;
        private string searchTerm;
        private string selectedSortOption;

        public TestListViewModel(ITestService testService)
        {
            this.testService = testService;
            this.TestCards = new ObservableCollection<TestCardDto>();
            this.SortOptions = new List<string> { "Name (A-Z)", "Name (Z-A)", "Level (Easy-Hard)", "Level (Hard-Easy)" };
            this.selectedSortOption = this.SortOptions.First();
            this.StartTestCommand = new RelayCommand(this.OnStartTest);
        }

        public event Action<string> NavigationRequested;

        public ObservableCollection<TestCardDto> TestCards { get; }

        public string SearchTerm
        {
            get => this.searchTerm;
            set
            {
                this.SetProperty(ref this.searchTerm, value);
                this.LoadDataAsync();
            }
        }

        public List<string> SortOptions { get; }

        public string SelectedSortOption
        {
            get => this.selectedSortOption;
            set
            {
                this.SetProperty(ref this.selectedSortOption, value);
                this.LoadDataAsync();
            }
        }

        public ICommand StartTestCommand { get; }

        public async Task LoadDataAsync()
        {
            if (this.isLoading)
            {
                return;
            }

            this.isLoading = true;
            try
            {
                string sortBy = this.selectedSortOption.Contains("Name") ? "Name" : "Level";
                bool ascending = this.selectedSortOption.Contains("(A-Z)") || this.selectedSortOption.Contains("(Easy-Hard)");
                var tests = await this.testService.GetPublishedTestsAsync(this.SearchTerm, sortBy, ascending);
                this.TestCards.Clear();
                foreach (var test in tests)
                {
                    this.TestCards.Add(test);
                }
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
                // NavigationRequested?.Invoke($"TestSession:{test.TestId}");
            }
        }
    }
}