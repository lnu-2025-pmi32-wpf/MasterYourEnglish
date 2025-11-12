using MasterYourEnglish.BLL.Interfaces;
using MasterYourEnglish.BLL.Models.DTOs;
using MasterYourEnglish.Presentation.ViewModels.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MasterYourEnglish.Presentation.ViewModels
{
    public class SavedFlashcardsViewModel : ViewModelBase, IPageViewModel
    {
        private readonly IFlashcardService _flashcardService;
        private readonly ICurrentUserService _currentUserService;
        private bool _isLoading = false;

        public ObservableCollection<SavedFlashcardDto> SavedFlashcards { get; }

        private SavedFlashcardDto _selectedFlashcard;
        public SavedFlashcardDto SelectedFlashcard
        {
            get => _selectedFlashcard;
            set => SetProperty(ref _selectedFlashcard, value);
        }

        private string _searchTerm;
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                SetProperty(ref _searchTerm, value);
                Task.Run(LoadDataAsync);
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

        // --- Commands ---
        public ICommand MarkAsLearnedCommand { get; }
        public ICommand RemoveFromSavedCommand { get; }

        public SavedFlashcardsViewModel(IFlashcardService flashcardService, ICurrentUserService currentUserService)
        {
            _flashcardService = flashcardService;
            _currentUserService = currentUserService;

            SavedFlashcards = new ObservableCollection<SavedFlashcardDto>();

            SortOptions = new List<string> { "Name (A-Z)", "Name (Z-A)", "Level (Easy-Hard)", "Level (Hard-Easy)" };
            _selectedSortOption = SortOptions.First();

            MarkAsLearnedCommand = new RelayCommand(p => OnMarkAsLearned(), p => SelectedFlashcard != null);
            RemoveFromSavedCommand = new RelayCommand(p => OnRemoveFromSaved(), p => SelectedFlashcard != null);
        }

        public async Task LoadDataAsync()
        {
            if (_isLoading) return;

            if (_currentUserService.CurrentUser == null) return;

            _isLoading = true;

            try
            {
                int userId = _currentUserService.CurrentUser.UserId;

                string sortBy = _selectedSortOption.Contains("Name") ? "Name" : "Level";
                bool ascending = _selectedSortOption.Contains("(A-Z)") || _selectedSortOption.Contains("(Easy-Hard)");

                var flashcards = await _flashcardService.GetSavedFlashcardsAsync(userId, SearchTerm, sortBy, ascending);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    SavedFlashcards.Clear();
                    foreach (var card in flashcards)
                    {
                        SavedFlashcards.Add(card);
                    }

                    if (SavedFlashcards.Count > 0)
                    {
                        SelectedFlashcard = SavedFlashcards[0];
                    }
                    else
                    {
                        SelectedFlashcard = null;
                    }
                });
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void OnMarkAsLearned()
        {
            OnRemoveFromSaved();
        }

        private async void OnRemoveFromSaved()
        {
            if (SelectedFlashcard == null) return;

            int userId = _currentUserService.CurrentUser.UserId;
            int cardId = SelectedFlashcard.FlashcardId;
            var cardToRemove = SelectedFlashcard;

            await _flashcardService.RemoveFromSavedAsync(userId, cardId);

            SavedFlashcards.Remove(cardToRemove);
            SelectedFlashcard = SavedFlashcards.FirstOrDefault();
        }
    }
}