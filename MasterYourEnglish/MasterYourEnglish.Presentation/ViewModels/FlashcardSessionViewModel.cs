namespace MasterYourEnglish.Presentation.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using MasterYourEnglish.BLL.DTOs;
    using MasterYourEnglish.BLL.Interfaces;
    using MasterYourEnglish.Presentation.ViewModels.Commands;

    public class FlashcardSessionViewModel : ViewModelBase
    {
        private readonly IFlashcardBundleService bundleService;
        private readonly IFlashcardService flashcardService;
        private readonly ICurrentUserService currentUserService;

        private List<FlashcardSessionDto> sessionCards;
        private Dictionary<int, bool> results;
        private int currentIndex;
        private int bundleId;
        private bool isFlipped;

        private FlashcardSessionDto currentFlashcard;
        private string progressText;

        public FlashcardSessionViewModel(
            IFlashcardBundleService bundleService,
            IFlashcardService flashcardService,
            ICurrentUserService currentUserService)
        {
            this.bundleService = bundleService;
            this.flashcardService = flashcardService;
            this.currentUserService = currentUserService;

            this.sessionCards = new List<FlashcardSessionDto>();
            this.results = new Dictionary<int, bool>();

            this.FlipCardCommand = new RelayCommand(p => this.IsFlipped = !this.IsFlipped);
            this.AddToSavedCommand = new RelayCommand(p => this.AddToSaved(), p => this.CanAddToSaved());
            this.KnowCommand = new RelayCommand(p => this.OnKnow());
            this.DontKnowCommand = new RelayCommand(p => this.OnDontKnow());
        }

        public event Action<string> NavigationRequested;

        public bool IsFlipped { get => this.isFlipped; set => this.SetProperty(ref this.isFlipped, value); }

        public FlashcardSessionDto CurrentFlashcard { get => this.currentFlashcard; set => this.SetProperty(ref this.currentFlashcard, value); }

        public string ProgressText { get => this.progressText; set => this.SetProperty(ref this.progressText, value); }

        public ICommand FlipCardCommand { get; }

        public ICommand KnowCommand { get; }

        public ICommand DontKnowCommand { get; }

        public ICommand AddToSavedCommand { get; }

        public async void LoadSession(int bundleId)
        {
            this.bundleId = bundleId;
            this.sessionCards = await this.bundleService.GetFlashcardSessionAsync(bundleId);
            this.StartSession();
        }

        public void LoadSession(List<FlashcardSessionDto> cards)
        {
            this.bundleId = 0;
            this.sessionCards = cards;
            this.StartSession();
        }

        public async void LoadSessionFromSaved()
        {
            this.bundleId = 0;
            int userId = this.currentUserService.CurrentUser.UserId;
            this.sessionCards = await this.flashcardService.GetSavedFlashcardsForSessionAsync(userId);
            this.StartSession();
        }

        private void StartSession()
        {
            if (this.sessionCards == null || this.sessionCards.Count == 0)
            {
                this.NavigationRequested?.Invoke("Flashcards");
                return;
            }

            this.currentIndex = 0;
            this.results.Clear();
            this.IsFlipped = false;
            this.UpdateCurrentCard();
        }

        private void UpdateCurrentCard()
        {
            this.CurrentFlashcard = this.sessionCards[this.currentIndex];
            this.ProgressText = $"{this.currentIndex + 1}/{this.sessionCards.Count}";
            CommandManager.InvalidateRequerySuggested();
        }

        private void OnKnow()
        {
            this.results[this.CurrentFlashcard.FlashcardId] = true;
            this.HandleAdvance();
        }

        private void OnDontKnow()
        {
            this.results[this.CurrentFlashcard.FlashcardId] = false;
            this.HandleAdvance();
        }

        private async void HandleAdvance()
        {
            if (this.currentIndex < this.sessionCards.Count - 1)
            {
                this.currentIndex++;
                this.IsFlipped = false;
                this.UpdateCurrentCard();
            }
            else
            {
                int userId = this.currentUserService.CurrentUser.UserId;

                if (this.bundleId > 0)
                {
                    await this.bundleService.SaveSessionAttemptAsync(this.bundleId, userId, this.results);
                }

                int knownCount = this.results.Count(r => r.Value == true);
                this.NavigationRequested?.Invoke($"SessionResults:{knownCount}:{this.sessionCards.Count}:Flashcards");
            }
        }

        private bool CanAddToSaved()
        {
            return this.CurrentFlashcard != null;
        }

        private async void AddToSaved()
        {
            if (!this.CanAddToSaved())
            {
                return;
            }

            int userId = this.currentUserService.CurrentUser.UserId;
            int flashcardId = this.CurrentFlashcard.FlashcardId;
            await this.flashcardService.AddToSavedAsync(userId, flashcardId);
        }
    }
}