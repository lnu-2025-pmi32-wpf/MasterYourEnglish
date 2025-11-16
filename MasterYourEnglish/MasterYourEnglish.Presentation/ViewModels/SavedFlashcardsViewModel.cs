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

    public class SavedFlashcardsViewModel : ViewModelBase, IPageViewModel
    {
        private readonly IFlashcardService flashcardService;
        private readonly ICurrentUserService currentUserService;
        private bool isLoading = false;
        private SavedFlashcardDto selectedFlashcard;
        private string searchTerm;
        private string selectedSortOption;

        public SavedFlashcardsViewModel(IFlashcardService flashcardService, ICurrentUserService currentUserService)
        {
            this.flashcardService = flashcardService;
            this.currentUserService = currentUserService;
            this.SavedFlashcards = new ObservableCollection<SavedFlashcardDto>();
            this.SortOptions = new List<string> { "Name (A-Z)", "Name (Z-A)", "Level (Easy-Hard)", "Level (Hard-Easy)" };
            this.selectedSortOption = this.SortOptions.First();
            this.MarkAsLearnedCommand = new RelayCommand(p => this.OnMarkAsLearned(), p => this.SelectedFlashcard != null);
            this.RemoveFromSavedCommand = new RelayCommand(p => this.OnRemoveFromSaved(), p => this.SelectedFlashcard != null);
            this.StartTestYourselfCommand = new RelayCommand(
                p => this.NavigationRequested?.Invoke("TestSavedFlashcards"));
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
                this.SetProperty(ref this.searchTerm, value);
                Task.Run(this.LoadDataAsync);
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

        public ICommand MarkAsLearnedCommand { get; }

        public ICommand RemoveFromSavedCommand { get; }

        public ICommand StartTestYourselfCommand { get; }

        public async Task LoadDataAsync()
        {
            if (this.isLoading)
            {
                return;
            }

            if (this.currentUserService.CurrentUser == null)
            {
                return;
            }

            this.isLoading = true;
            try
            {
                int userId = this.currentUserService.CurrentUser.UserId;
                string sortBy = this.selectedSortOption.Contains("Name") ? "Name" : "Level";
                bool ascending = this.selectedSortOption.Contains("(A-Z)") || this.selectedSortOption.Contains("(Easy-Hard)");
                var flashcards = await this.flashcardService.GetSavedFlashcardsAsync(userId, this.SearchTerm, sortBy, ascending);
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
            finally
            {
                this.isLoading = false;
            }
        }

        private void OnMarkAsLearned()
        {
            this.OnRemoveFromSaved();
        }

        private async void OnRemoveFromSaved()
        {
            if (this.SelectedFlashcard == null)
            {
                return;
            }

            int userId = this.currentUserService.CurrentUser.UserId;
            int cardId = this.SelectedFlashcard.FlashcardId;
            var cardToRemove = this.SelectedFlashcard;

            await this.flashcardService.RemoveFromSavedAsync(userId, cardId);

            this.SavedFlashcards.Remove(cardToRemove);
            this.SelectedFlashcard = this.SavedFlashcards.FirstOrDefault();
        }
    }
}
