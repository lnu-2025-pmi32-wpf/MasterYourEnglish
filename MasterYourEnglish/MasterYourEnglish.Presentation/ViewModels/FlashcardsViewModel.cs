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

    public class FlashcardsViewModel : ViewModelBase, IPageViewModel
    {
        private readonly IFlashcardBundleService bundleService;
        private bool isLoading = false;
        private string searchTerm;
        private string selectedSortOption;

        public FlashcardsViewModel(IFlashcardBundleService bundleService)
        {
            this.bundleService = bundleService;
            this.FlashcardBundles = new ObservableCollection<FlashcardBundleCardDto>();
            this.SortOptions = new List<string> { "Name (A-Z)", "Name (Z-A)", "Level (Easy-Hard)", "Level (Hard-Easy)" };
            this.selectedSortOption = this.SortOptions.First();
            this.NavigateToSavedCommand = new RelayCommand(p => this.NavigationRequested?.Invoke("SavedFlashcards"));
            this.StartSessionCommand = new RelayCommand(this.OnStartSession);
            this.NavigateToGenerateCommand = new RelayCommand(p => this.NavigationRequested?.Invoke("GenerateBundle"));
            this.NavigateToCreateCommand = new RelayCommand(p => this.NavigationRequested?.Invoke("CreateBundle"));
        }

        public event Action<string> NavigationRequested;

        public ObservableCollection<FlashcardBundleCardDto> FlashcardBundles { get; }

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

        public ICommand NavigateToSavedCommand { get; }

        public ICommand StartSessionCommand { get; }

        public ICommand NavigateToGenerateCommand { get; }

        public ICommand NavigateToCreateCommand { get; }

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
                var bundles = await this.bundleService.GetPublishedBundlesAsync(this.SearchTerm, sortBy, ascending);
                this.FlashcardBundles.Clear();
                foreach (var bundle in bundles)
                {
                    this.FlashcardBundles.Add(bundle);
                }
            }
            finally
            {
                this.isLoading = false;
            }
        }

        private void OnStartSession(object parameter)
        {
            if (parameter is FlashcardBundleCardDto bundle)
            {
                this.NavigationRequested?.Invoke($"FlashcardSession:{bundle.BundleId}");
            }
        }
    }
}