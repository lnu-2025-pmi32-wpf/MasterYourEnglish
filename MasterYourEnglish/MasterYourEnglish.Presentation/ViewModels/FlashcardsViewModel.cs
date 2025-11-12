using MasterYourEnglish.BLL.Interfaces;
using MasterYourEnglish.BLL.Models.DTOs;
using MasterYourEnglish.Presentation.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace MasterYourEnglish.Presentation.ViewModels
{
    public class FlashcardsViewModel : ViewModelBase
    {
        // This is our "fake database"
        private readonly List<FlashcardBundleCardDto> _allFakeBundles;

        // This is the list the UI is bound to
        public ObservableCollection<FlashcardBundleCardDto> FlashcardBundles { get; }

        // This event tells the MainWindow to navigate
        public event Action<string> NavigationRequested;

        // --- Property for Search ---
        private string _searchTerm;
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                SetProperty(ref _searchTerm, value);
                LoadBundles(); // Re-run the filter when text changes
            }
        }

        // --- Properties for Sorting ---
        public List<string> SortOptions { get; }
        private string _selectedSortOption;
        public string SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                SetProperty(ref _selectedSortOption, value);
                LoadBundles(); // Re-run the filter/sort
            }
        }

        // --- Commands ---
        public ICommand NavigateToSavedCommand { get; }
        public ICommand StartSessionCommand { get; }

        public FlashcardsViewModel(/*IFlashcardBundleService bundleService*/)
        {
            // _bundleService = bundleService;

            // 1. Create the master list ONE time
            _allFakeBundles = new List<FlashcardBundleCardDto>();
            CreateFakeDatabase();

            // 2. Create the UI list
            FlashcardBundles = new ObservableCollection<FlashcardBundleCardDto>();

            // 3. Setup sorting
            SortOptions = new List<string> { "Name (A-Z)", "Name (Z-A)", "Level (Easy-Hard)", "Level (Hard-Easy)" };
            _selectedSortOption = SortOptions.First();

            // 4. Setup commands
            NavigateToSavedCommand = new RelayCommand(p => NavigationRequested?.Invoke("SavedFlashcards"));
            StartSessionCommand = new RelayCommand(OnStartSession);

            // 5. Load the initial data
            LoadBundles();
        }
        private void LoadBundles()
        {
            // 1. Get the base list
            IEnumerable<FlashcardBundleCardDto> bundles = _allFakeBundles;

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                string lowerSearch = SearchTerm.ToLower();
                bundles = bundles.Where(b => b.BundleName.ToLower().Contains(lowerSearch) ||
                                             b.CategoryName.ToLower().Contains(lowerSearch));
            }

            string sortBy = _selectedSortOption.Contains("Name") ? "Name" : "Level";
            bool ascending = _selectedSortOption.Contains("(A-Z)") || _selectedSortOption.Contains("(Easy-Hard)");

            switch (sortBy)
            {
                case "Level":
                    bundles = ascending
                        ? bundles.OrderBy(b => b.CategoryName) // Assumes CategoryName is the level
                        : bundles.OrderByDescending(b => b.CategoryName);
                    break;
                case "Name":
                default:
                    bundles = ascending
                        ? bundles.OrderBy(b => b.BundleName)
                        : bundles.OrderByDescending(b => b.BundleName);
                    break;
            }

            // 4. THIS IS THE FIX. Clear the list before adding new items.
            FlashcardBundles.Clear();
            foreach (var bundle in bundles)
            {
                FlashcardBundles.Add(bundle);
            }
        }

        private void OnStartSession(object parameter)
        {
            if (parameter is FlashcardBundleCardDto bundle)
            {
                NavigationRequested?.Invoke($"FlashcardSession:{bundle.BundleId}");
            }
        }

        private void CreateFakeDatabase()
        {
            _allFakeBundles.Add(new FlashcardBundleCardDto { BundleId = 1, CategoryName = "B1 Level", BundleName = "Common Verbs" });
            _allFakeBundles.Add(new FlashcardBundleCardDto { BundleId = 2, CategoryName = "A2 Level", BundleName = "Food & Drink" });
            _allFakeBundles.Add(new FlashcardBundleCardDto { BundleId = 3, CategoryName = "C1 Level", BundleName = "Business Idioms" });
            _allFakeBundles.Add(new FlashcardBundleCardDto { BundleId = 4, CategoryName = "B2 Level", BundleName = "Travel Phrasals" });
            _allFakeBundles.Add(new FlashcardBundleCardDto { BundleId = 5, CategoryName = "A1 Level", BundleName = "Basic Greetings" });
            _allFakeBundles.Add(new FlashcardBundleCardDto { BundleId = 6, CategoryName = "B2 Level", BundleName = "Medical Terms" });
        }
    }
}