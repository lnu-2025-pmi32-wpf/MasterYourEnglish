using MasterYourEnglish.BLL.DTOs;
using MasterYourEnglish.BLL.Interfaces;
using MasterYourEnglish.BLL.Models.DTOs;
using MasterYourEnglish.Presentation.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MasterYourEnglish.Presentation.ViewModels
{
    public class TestListViewModel : ViewModelBase, IPageViewModel
    {
        private readonly ITestService _testService;
        private bool _isLoading = false;

        // --- Data for UI ---
        public ObservableCollection<TestCardDto> TestCards { get; }

        // --- Navigation Event ---
        public event Action<string> NavigationRequested;

        // --- Search & Sort ---
        private string _searchTerm;
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                SetProperty(ref _searchTerm, value);
                LoadDataAsync();
            }
        }

        public List<string> SortOptions { get; }
        private string _selectedSortOption;
        public string SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                SetProperty(ref _selectedSortOption, value);
                LoadDataAsync();
            }
        }

        public ICommand NavigateToCreateCommand { get; }
        public ICommand NavigateToGenerateCommand { get; }
        public ICommand StartTestCommand { get; }

        public TestListViewModel(ITestService testService)
        {
            _testService = testService;
            TestCards = new ObservableCollection<TestCardDto>();

            SortOptions = new List<string> { "Name (A-Z)", "Name (Z-A)", "Level (Easy-Hard)", "Level (Hard-Easy)" };
            _selectedSortOption = SortOptions.First();

            NavigateToCreateCommand = new RelayCommand(p => NavigationRequested?.Invoke("CreateTest"));
            NavigateToGenerateCommand = new RelayCommand(p => NavigationRequested?.Invoke("GenerateTest"));
            StartTestCommand = new RelayCommand(OnStartTest);
        }

        public async Task LoadDataAsync()
        {
            if (_isLoading) return;
            _isLoading = true;

            try
            {
                string sortBy = _selectedSortOption.Contains("Name") ? "Name" : "Level";
                bool ascending = _selectedSortOption.Contains("(A-Z)") || _selectedSortOption.Contains("(Easy-Hard)");

                var tests = await _testService.GetPublishedTestsAsync(SearchTerm, sortBy, ascending);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    TestCards.Clear();
                    foreach (var test in tests)
                    {
                        TestCards.Add(test);
                    }
                });
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void OnStartTest(object parameter)
        {
            if (parameter is TestCardDto test)
            {
                NavigationRequested?.Invoke($"TestSession:{test.TestId}");
            }
        }
    }
}