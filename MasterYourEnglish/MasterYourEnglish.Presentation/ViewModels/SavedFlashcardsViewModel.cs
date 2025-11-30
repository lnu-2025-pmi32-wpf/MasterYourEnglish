namespace MasterYourEnglish.Presentation.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using MasterYourEnglish.BLL.Interfaces;
    using MasterYourEnglish.BLL.Models.DTOs;
    using MasterYourEnglish.Presentation.ViewModels.Commands;
    using Microsoft.Extensions.Logging;

    public class SavedFlashcardsViewModel : ViewModelBase, IPageViewModel
    {
        private readonly IFlashcardService flashcardService;
        private readonly ICurrentUserService currentUserService;
        private readonly ILogger<SavedFlashcardsViewModel> logger;
        private bool isLoading = false;
        private SavedFlashcardDto selectedFlashcard;
        private string searchTerm;
        private string selectedSortOption;

        public SavedFlashcardsViewModel(
            IFlashcardService flashcardService,
            ICurrentUserService currentUserService,
            ILogger<SavedFlashcardsViewModel> logger)
        {
            this.flashcardService = flashcardService;
            this.currentUserService = currentUserService;
            this.logger = logger;

            this.SavedFlashcards = new ObservableCollection<SavedFlashcardDto>();
            this.SortOptions = new List<string> { "Name (A-Z)", "Name (Z-A)", "Level (Easy-Hard)", "Level (Hard-Easy)" };
            this.selectedSortOption = this.SortOptions.First();
            this.MarkAsLearnedCommand = new RelayCommand(p => this.OnMarkAsLearned(), p => this.SelectedFlashcard != null);
            this.RemoveFromSavedCommand = new RelayCommand(p => this.OnRemoveFromSaved(), p => this.SelectedFlashcard != null);
            this.StartTestYourselfCommand = new RelayCommand(
                p => this.NavigationRequested?.Invoke("TestSavedFlashcards"));

            this.logger.Log(LogLevel.Information, "SavedFlashcardsViewModel initialized.");
        }

        public event Action<string> NavigationRequested;

        public ObservableCollection<SavedFlashcardDto> SavedFlashcards { get; }

        public SavedFlashcardDto SelectedFlashcard
        {
            get => this.selectedFlashcard;
            set => this.SetProperty(ref this.selectedFlashcard, value);
        }

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

        public ICommand MarkAsLearnedCommand { get; }

        public ICommand RemoveFromSavedCommand { get; }

        public ICommand StartTestYourselfCommand { get; }

        public async Task LoadDataAsync()
        {
            if (this.isLoading)
            {
                this.logger.Log(LogLevel.Debug, "LoadDataAsync skipped, already loading.");
                return;
            }

            if (this.currentUserService.CurrentUser == null)
            {
                this.logger.Log(LogLevel.Warning, "LoadDataAsync skipped, current user is null.");
                return;
            }

            this.isLoading = true;
            int userId = this.currentUserService.CurrentUser.UserId;
            this.logger.Log(LogLevel.Information, "Starting to load saved flashcards for user ID: {Id}", userId);

            try
            {
                string sortBy = this.selectedSortOption.Contains("Name") ? "Name" : "Level";
                bool ascending = this.selectedSortOption.Contains("(A-Z)") || this.selectedSortOption.Contains("(Easy-Hard)");

                var flashcards = await this.flashcardService.GetSavedFlashcardsAsync(userId, this.searchTerm, sortBy, ascending);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.SavedFlashcards.Clear();
                    foreach (var card in flashcards)
                    {
                        this.SavedFlashcards.Add(card);
                    }

                    if (this.SavedFlashcards.Count > 0)
                    {
                        this.SelectedFlashcard = this.SavedFlashcards[0];
                    }
                    else
                    {
                        this.SelectedFlashcard = null;
                    }
                });
            }
            catch (Exception ex)
            {
                this.logger.Log(LogLevel.Error, ex, "Failed to load saved flashcards for user ID: {Id}", userId);
            }
            finally
            {
                this.isLoading = false;
            }
        }

        private void OnMarkAsLearned()
        {
            this.logger.Log(LogLevel.Information, "Marking card as learned (removing from saved).");
            this.OnRemoveFromSaved();
        }

        private async void OnRemoveFromSaved()
        {
            if (this.SelectedFlashcard == null)
            {
                this.logger.Log(LogLevel.Warning, "Attempted to remove card, but SelectedFlashcard is null.");
                return;
            }

            int userId = this.currentUserService.CurrentUser.UserId;
            int cardId = this.SelectedFlashcard.FlashcardId;
            var cardToRemove = this.SelectedFlashcard;

            this.logger.Log(LogLevel.Information, "Removing saved flashcard ID {CardId} for user {UserId}.", cardId, userId);
            try
            {
                await this.flashcardService.RemoveFromSavedAsync(userId, cardId);

                this.SavedFlashcards.Remove(cardToRemove);
                this.SelectedFlashcard = this.SavedFlashcards.FirstOrDefault();
                this.logger.Log(LogLevel.Information, "Flashcard ID {CardId} successfully removed from saved list.", cardId);
            }
            catch (Exception ex)
            {
                this.logger.Log(LogLevel.Error, ex, "Failed to remove flashcard ID {CardId} from saved list.", cardId);
            }
        }
    }
}