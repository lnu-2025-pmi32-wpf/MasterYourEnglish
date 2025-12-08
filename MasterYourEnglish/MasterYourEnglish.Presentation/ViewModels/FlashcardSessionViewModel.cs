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
    using Microsoft.Extensions.Logging;

    public class FlashcardSessionViewModel : ViewModelBase
    {
        private readonly IFlashcardBundleService bundleService;
        private readonly IFlashcardService flashcardService;
        private readonly ICurrentUserService currentUserService;
        private readonly ILogger<FlashcardSessionViewModel> logger;

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
            ICurrentUserService currentUserService,
            ILogger<FlashcardSessionViewModel> logger)
        {
            this.bundleService = bundleService;
            this.flashcardService = flashcardService;
            this.currentUserService = currentUserService;
            this.logger = logger;

            this.sessionCards = new List<FlashcardSessionDto>();
            this.results = new Dictionary<int, bool>();

            this.FlipCardCommand = new RelayCommand(p => this.IsFlipped = !this.IsFlipped);
            this.AddToSavedCommand = new RelayCommand(p => this.AddToSaved(), p => this.CanAddToSaved());
            this.KnowCommand = new RelayCommand(p => this.OnKnow());
            this.DontKnowCommand = new RelayCommand(p => this.OnDontKnow());
            this.logger.LogInformation("Flashcard Session view model initialized.");
        }

        public event Action<string> NavigationRequested;

        public bool IsFlipped { get => this.isFlipped; set => this.SetProperty(ref this.isFlipped, value); }

        public FlashcardSessionDto CurrentFlashcard { get => this.currentFlashcard; set => this.SetProperty(ref this.currentFlashcard, value); }

        public string ProgressText { get => this.progressText; set => this.SetProperty(ref this.progressText, value); }
        
        private string sessionTitle = "Session";

        public string SessionTitle { get => this.sessionTitle; set => this.SetProperty(ref this.sessionTitle, value); }

        public ICommand FlipCardCommand { get; }

        public ICommand KnowCommand { get; }

        public ICommand DontKnowCommand { get; }

        public ICommand AddToSavedCommand { get; }

        public async void LoadSession(int bundleId)
        {
            this.logger.LogInformation("Loading session for bundle ID: {Id}", bundleId);
            try
            {
                this.bundleId = bundleId;
                
                // Fetch bundle details for the header
                var bundle = await this.bundleService.GetBundleByIdAsync(bundleId);
                if (bundle != null)
                {
                    this.SessionTitle = bundle.BundleName;
                }
                else
                {
                    this.SessionTitle = "Session";
                }

                this.sessionCards = await this.bundleService.GetFlashcardSessionAsync(bundleId);
                this.StartSession();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to load session for bundle ID: {Id}", bundleId);
                this.SessionTitle = "Session";
            }
        }

        public void LoadSession(List<FlashcardSessionDto> cards)
        {
            this.bundleId = 0;
            this.SessionTitle = "Generated Session";
            this.sessionCards = cards;
            this.logger.LogInformation("Loading session from generated cards. Card Count: {Count}", this.sessionCards.Count);
            this.StartSession();
        }

        public async void LoadSessionFromSaved()
        {
            this.logger.LogInformation("Loading session from saved flashcards.");
            try
            {
                this.bundleId = 0;
                this.SessionTitle = "Saved Cards";
                int userId = this.currentUserService.CurrentUser.UserId;
                this.sessionCards = await this.flashcardService.GetSavedFlashcardsForSessionAsync(userId);
                this.StartSession();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to load session from saved flashcards.");
                this.SessionTitle = "Session";
            }
        }

        private void StartSession()
        {
            if (this.sessionCards == null || this.sessionCards.Count == 0)
            {
                this.logger.LogWarning("Session started with zero cards. Redirecting to Flashcards view.");
                this.NavigationRequested?.Invoke("Flashcards");
                return;
            }

            this.currentIndex = 0;
            this.results.Clear();
            this.IsFlipped = false;
            this.UpdateCurrentCard();
            this.logger.LogInformation("Session successfully started with {Count} cards.", this.sessionCards.Count);
        }

        private void UpdateCurrentCard()
        {
            this.CurrentFlashcard = this.sessionCards[this.currentIndex];
            this.ProgressText = $"Flashcard {this.currentIndex + 1} of {this.sessionCards.Count}";
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
                this.logger.LogInformation("Session finished. Processing results.");
                int userId = this.currentUserService.CurrentUser.UserId;

                if (this.bundleId > 0)
                {
                    try
                    {
                        await this.bundleService.SaveSessionAttemptAsync(this.bundleId, userId, this.results);
                        this.logger.LogInformation("Session attempt saved for bundle ID: {Id}", this.bundleId);
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogError(ex, "Failed to save session attempt for bundle ID: {Id}", this.bundleId);
                    }
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
                this.logger.LogWarning("Attempted to add card to saved list, but CurrentFlashcard is null.");
                return;
            }

            int userId = this.currentUserService.CurrentUser.UserId;
            int flashcardId = this.CurrentFlashcard.FlashcardId;
            this.logger.LogInformation("Attempting to add flashcard ID {CardId} to saved list for user {UserId}.", flashcardId, userId);

            try
            {
                await this.flashcardService.AddToSavedAsync(userId, flashcardId);
                this.logger.LogInformation("Flashcard ID {CardId} successfully added to saved list.", flashcardId);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to add flashcard ID {CardId} to saved list.", flashcardId);
            }
        }
    }
}