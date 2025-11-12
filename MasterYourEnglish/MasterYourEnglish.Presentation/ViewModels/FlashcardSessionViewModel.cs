using MasterYourEnglish.BLL.DTOs;
using MasterYourEnglish.BLL.Interfaces;
using MasterYourEnglish.Presentation.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MasterYourEnglish.Presentation.ViewModels
{
    public class FlashcardSessionViewModel : ViewModelBase
    {
        private readonly IFlashcardBundleService _bundleService;
        private readonly IFlashcardService _flashcardService;
        private readonly ICurrentUserService _currentUserService;

        private List<FlashcardSessionDto> _sessionCards;
        private Dictionary<int, bool> _results;
        private int _currentIndex;
        private int _bundleId; 
        private bool _isFlipped;
        public bool IsFlipped { get => _isFlipped; set => SetProperty(ref _isFlipped, value); }
        private FlashcardSessionDto _currentFlashcard;
        public FlashcardSessionDto CurrentFlashcard { get => _currentFlashcard; set => SetProperty(ref _currentFlashcard, value); }
        private string _progressText;
        public string ProgressText { get => _progressText; set => SetProperty(ref _progressText, value); }


        public event Action<string> NavigationRequested;
        public ICommand FlipCardCommand { get; }
        public ICommand KnowCommand { get; }
        public ICommand DontKnowCommand { get; }
        public ICommand AddToSavedCommand { get; }

        public FlashcardSessionViewModel(
            IFlashcardBundleService bundleService,
            IFlashcardService flashcardService,
            ICurrentUserService currentUserService)
        {
            _bundleService = bundleService;
            _flashcardService = flashcardService;
            _currentUserService = currentUserService;

            _sessionCards = new List<FlashcardSessionDto>();
            _results = new Dictionary<int, bool>();

            FlipCardCommand = new RelayCommand(p => IsFlipped = !IsFlipped);
            AddToSavedCommand = new RelayCommand(p => AddToSaved(), p => CanAddToSaved());
            KnowCommand = new RelayCommand(p => OnKnow());
            DontKnowCommand = new RelayCommand(p => OnDontKnow());
        }

        public async void LoadSession(int bundleId)
        {
            _bundleId = bundleId;
            _sessionCards = await _bundleService.GetFlashcardSessionAsync(bundleId);
            StartSession();
        }

        public async void LoadSessionFromSaved()
        {
            _bundleId = 0;
            int userId = _currentUserService.CurrentUser.UserId;
            _sessionCards = await _flashcardService.GetSavedFlashcardsForSessionAsync(userId);
            StartSession();
        }

        private void StartSession()
        {
            if (_sessionCards == null || _sessionCards.Count == 0)
            {
                NavigationRequested?.Invoke("Flashcards"); 
                return;
            }

            _currentIndex = 0;
            _results.Clear();
            IsFlipped = false;
            UpdateCurrentCard();
        }

        private void UpdateCurrentCard()
        {
            CurrentFlashcard = _sessionCards[_currentIndex];
            ProgressText = $"{_currentIndex + 1}/{_sessionCards.Count}";
            CommandManager.InvalidateRequerySuggested();
        }

        private void OnKnow()
        {
            _results[CurrentFlashcard.FlashcardId] = true;
            HandleAdvance();
        }

        private void OnDontKnow()
        {
            _results[CurrentFlashcard.FlashcardId] = false;
            HandleAdvance();
        }

        private async void HandleAdvance()
        {
            if (_currentIndex < _sessionCards.Count - 1)
            {
                _currentIndex++;
                IsFlipped = false;
                UpdateCurrentCard();
            }
            else
            {
                int userId = _currentUserService.CurrentUser.UserId;

                if (_bundleId > 0)
                {
                    await _bundleService.SaveSessionAttemptAsync(_bundleId, userId, _results);
                }

                int knownCount = _results.Count(r => r.Value == true);
                NavigationRequested?.Invoke($"SessionResults:{knownCount}:{_sessionCards.Count}");
            }
        }

        private bool CanAddToSaved()
        {
            return CurrentFlashcard != null;
        }

        private async void AddToSaved()
        {
            if (!CanAddToSaved()) return;
            int userId = _currentUserService.CurrentUser.UserId;
            int flashcardId = CurrentFlashcard.FlashcardId;
            await _flashcardService.AddToSavedAsync(userId, flashcardId);
        }
    }
}