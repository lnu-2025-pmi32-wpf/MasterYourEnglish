using MasterYourEnglish.BLL.Interfaces;
using MasterYourEnglish.BLL.Models.DTOs;
using MasterYourEnglish.Presentation.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MasterYourEnglish.Presentation.ViewModels
{
    public class FlashcardsViewModel : ViewModelBase, IPageViewModel 
    {
        private readonly IFlashcardBundleService _bundleService;
        private bool _isLoading = false;

        public ObservableCollection<FlashcardBundleCardDto> FlashcardBundles { get; }
        public event Action<string> NavigationRequested;

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

        public ICommand NavigateToSavedCommand { get; }
        public ICommand StartSessionCommand { get; }
        public ICommand NavigateToGenerateCommand { get; }
        public ICommand NavigateToCreateCommand { get; }

        public FlashcardsViewModel(IFlashcardBundleService bundleService)
        {
            _bundleService = bundleService;
            FlashcardBundles = new ObservableCollection<FlashcardBundleCardDto>();

            SortOptions = new List<string> { "Name (A-Z)", "Name (Z-A)", "Level (Easy-Hard)", "Level (Hard-Easy)" };
            _selectedSortOption = SortOptions.First();

            NavigateToSavedCommand = new RelayCommand(p => NavigationRequested?.Invoke("SavedFlashcards"));
            StartSessionCommand = new RelayCommand(OnStartSession);
            NavigateToGenerateCommand = new RelayCommand(p => NavigationRequested?.Invoke("GenerateBundle"));
            NavigateToCreateCommand = new RelayCommand(p => NavigationRequested?.Invoke("CreateBundle"));

        }

        public async Task LoadDataAsync()
        {
            if (_isLoading) return;
            _isLoading = true;

            try
            {
                string sortBy = _selectedSortOption.Contains("Name") ? "Name" : "Level";
                bool ascending = _selectedSortOption.Contains("(A-Z)") || _selectedSortOption.Contains("(Easy-Hard)");

                var bundles = await _bundleService.GetPublishedBundlesAsync(SearchTerm, sortBy, ascending);

                FlashcardBundles.Clear();
                foreach (var bundle in bundles)
                {
                    FlashcardBundles.Add(bundle);
                }
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void OnStartSession(object parameter)
        {
            if (parameter is FlashcardBundleCardDto bundle)
            {
                NavigationRequested?.Invoke($"FlashcardSession:{bundle.BundleId}");
            }
        }
    }
}