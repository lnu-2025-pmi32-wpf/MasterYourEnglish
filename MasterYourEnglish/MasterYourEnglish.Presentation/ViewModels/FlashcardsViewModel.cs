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
    using Microsoft.Extensions.Logging;

    public class FlashcardsViewModel : ViewModelBase, IPageViewModel
    {
        private readonly IFlashcardBundleService bundleService;
        private readonly ILogger<FlashcardsViewModel> logger;
        private bool isLoading = false;
        private string searchTerm;
        private string selectedSortOption;

        public FlashcardsViewModel(
            IFlashcardBundleService bundleService,
            ILogger<FlashcardsViewModel> logger)
        {
            this.bundleService = bundleService;
            this.logger = logger;

            this.FlashcardBundles = new ObservableCollection<FlashcardBundleCardDto>();
            this.SortOptions = new List<string> { "Name (A-Z)", "Name (Z-A)", "Level (Easy-Hard)", "Level (Hard-Easy)" };
            this.selectedSortOption = this.SortOptions.First();

            this.NavigateToSavedCommand = new RelayCommand(p => this.NavigationRequested?.Invoke("SavedFlashcards"));
            this.StartSessionCommand = new RelayCommand(this.OnStartSession);
            this.NavigateToGenerateCommand = new RelayCommand(p => this.NavigationRequested?.Invoke("GenerateBundle"));
            this.NavigateToCreateCommand = new RelayCommand(p => this.NavigationRequested?.Invoke("CreateBundle"));

            this.logger.LogInformation("FlashcardsViewModel initialized.");
        }

        public event Action<string> NavigationRequested;

        public ObservableCollection<FlashcardBundleCardDto> FlashcardBundles { get; }

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

        public ICommand NavigateToSavedCommand { get; }

        public ICommand StartSessionCommand { get; }

        public ICommand NavigateToGenerateCommand { get; }

        public ICommand NavigateToCreateCommand { get; }

        public async Task LoadDataAsync()
        {
            if (this.isLoading)
            {
                this.logger.LogDebug("LoadDataAsync skipped, already loading.");
                return;
            }

            this.isLoading = true;

            this.logger.LogInformation(
                "Starting to load published bundles. SearchTerm: '{SearchTerm}', Sort: {SortOption}",
                this.searchTerm,
                this.selectedSortOption);

            try
            {
                string sortBy = this.selectedSortOption.Contains("Name") ? "Name" : "Level";
                bool ascending = this.selectedSortOption.Contains("(A-Z)") || this.selectedSortOption.Contains("(Easy-Hard)");

                var bundles = await this.bundleService.GetPublishedBundlesAsync(this.searchTerm, sortBy, ascending);

                this.FlashcardBundles.Clear();
                foreach (var bundle in bundles)
                {
                    this.FlashcardBundles.Add(bundle);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to load published bundles.");
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
                this.logger.LogInformation("Starting session for bundle ID: {Id}", bundle.BundleId);
                this.NavigationRequested?.Invoke($"FlashcardSession:{bundle.BundleId}");
            }
            else
            {
                this.logger.LogWarning("Attempted to start session without selecting a valid bundle.");
            }
        }
    }
}
