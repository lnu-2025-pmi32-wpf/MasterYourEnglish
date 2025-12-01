namespace MasterYourEnglish.Presentation.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using MasterYourEnglish.BLL.DTOs;
    using MasterYourEnglish.BLL.Interfaces;
    using MasterYourEnglish.BLL.Models.DTOs;
    using MasterYourEnglish.Presentation.ViewModels.Commands;
    using Microsoft.Extensions.Logging;

    public class CreateBundleViewModel : ViewModelBase, IPageViewModel
    {
        private readonly ITopicService topicService;
        private readonly IFlashcardBundleService bundleService;
        private readonly ICurrentUserService currentUserService;
        private readonly ILogger<CreateBundleViewModel> logger;

        private string title = "My New Bundle";
        private string description = string.Empty;
        private TopicDto selectedTopic;
        private string difficulty = "B2";
        private string newWord;
        private string newMeaning;
        private string newExample;
        private string newTranscription;
        private string newPartOfSpeech;

        public CreateBundleViewModel(
            ITopicService topicService,
            IFlashcardBundleService bundleService,
            ICurrentUserService currentUserService,
            ILogger<CreateBundleViewModel> logger)
        {
            this.topicService = topicService;
            this.bundleService = bundleService;
            this.currentUserService = currentUserService;
            this.logger = logger;

            this.NewFlashcards = new ObservableCollection<CreateFlashcardDto>();
            this.AvailableTopics = new ObservableCollection<TopicDto>();

            this.AddCardCommand = new RelayCommand(p => this.OnAddCard(), p => this.CanAddCard());
            this.RemoveCardCommand = new RelayCommand(this.OnRemoveCard);
            this.SaveBundleCommand = new RelayCommand(async p => await this.OnSaveBundle(), p => this.CanSaveBundle());
            this.CancelCommand = new RelayCommand(p => this.NavigationRequested?.Invoke("Flashcards"));

            this.logger.LogInformation("CreateBundleViewModel initialized.");
        }

        public event Action<string> NavigationRequested;

        public string Title { get => this.title; set => this.SetProperty(ref this.title, value); }

        public string Description { get => this.description; set => this.SetProperty(ref this.description, value); }

        public TopicDto SelectedTopic { get => this.selectedTopic; set => this.SetProperty(ref this.selectedTopic, value); }

        public ObservableCollection<TopicDto> AvailableTopics { get; }

        public string Difficulty { get => this.difficulty; set => this.SetProperty(ref this.difficulty, value); }

        public string NewWord { get => this.newWord; set => this.SetProperty(ref this.newWord, value); }

        public string NewMeaning { get => this.newMeaning; set => this.SetProperty(ref this.newMeaning, value); }

        public string NewExample { get => this.newExample; set => this.SetProperty(ref this.newExample, value); }

        public string NewTranscription { get => this.newTranscription; set => this.SetProperty(ref this.newTranscription, value); }

        public string NewPartOfSpeech { get => this.newPartOfSpeech; set => this.SetProperty(ref this.newPartOfSpeech, value); }

        public ObservableCollection<CreateFlashcardDto> NewFlashcards { get; }

        public ICommand AddCardCommand { get; }

        public ICommand RemoveCardCommand { get; }

        public ICommand SaveBundleCommand { get; }

        public ICommand CancelCommand { get; }

        public async Task LoadDataAsync()
        {
            this.logger.LogInformation("Loading available topics for bundle creation.");
            try
            {
                var topicDtos = await this.topicService.GetAllTopicsAsync();
                this.AvailableTopics.Clear();
                foreach (var dto in topicDtos)
                {
                    this.AvailableTopics.Add(dto);
                }

                this.SelectedTopic = this.AvailableTopics.FirstOrDefault();
                this.logger.LogInformation("Successfully loaded {Count} topics.", topicDtos.Count);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to load topics for bundle creation.");
            }
        }

        private bool CanAddCard() => !string.IsNullOrWhiteSpace(this.NewWord) && !string.IsNullOrWhiteSpace(this.NewMeaning);

        private void OnAddCard()
        {
            if (!this.CanAddCard())
            {
                this.logger.LogWarning("Cannot add card: Word or Meaning is missing.");
                return;
            }

            var newCard = new CreateFlashcardDto
            {
                Word = this.NewWord,
                Meaning = this.NewMeaning,
                Example = this.NewExample ?? string.Empty,
                DifficultyLevel = this.Difficulty,
                Transcription = this.NewTranscription ?? string.Empty,
                PartOfSpeech = this.NewPartOfSpeech ?? string.Empty,
            };

            this.NewFlashcards.Add(newCard);
            this.logger.LogDebug("New card added: {Word}", newCard.Word);

            // Скидання полів
            this.NewWord = string.Empty;
            this.NewMeaning = string.Empty;
            this.NewExample = string.Empty;
            this.NewTranscription = string.Empty;
            this.NewPartOfSpeech = string.Empty;
        }

        private void OnRemoveCard(object parameter)
        {
            if (parameter is CreateFlashcardDto card)
            {
                this.NewFlashcards.Remove(card);
                this.logger.LogInformation("Card removed: {Word}", card.Word);
            }
            else
            {
                this.logger.LogWarning("Attempted to remove card with invalid parameter.");
            }
        }

        private bool CanSaveBundle() => this.NewFlashcards.Count > 0 && !string.IsNullOrWhiteSpace(this.Title) && this.SelectedTopic != null;

        private async Task OnSaveBundle()
        {
            if (!this.CanSaveBundle())
            {
                this.logger.LogWarning("Save failed: Bundle data is incomplete or flashcards list is empty.");
                return;
            }

            this.logger.LogInformation("Attempting to save new bundle: {Title}", this.Title);

            try
            {
                var newBundleDto = new CreateBundleDto
                {
                    Title = this.Title,
                    Description = this.Description ?? string.Empty,
                    TopicId = this.SelectedTopic.TopicId,
                    DifficultyLevel = this.Difficulty,
                    NewFlashcards = this.NewFlashcards.ToList(),
                };

                int userId = this.currentUserService.CurrentUser.UserId;

                int newBundleId = await this.bundleService.CreateNewBundleAsync(newBundleDto, userId);

                this.logger.LogInformation("Bundle '{Title}' created successfully with ID: {Id}", this.Title, newBundleId);

                // Скидання полів після успішного збереження
                this.Title = "My New Bundle";
                this.Description = string.Empty;
                this.Difficulty = "B2";
                this.NewFlashcards.Clear();
                if (this.AvailableTopics.Count > 0)
                {
                    this.SelectedTopic = this.AvailableTopics.FirstOrDefault();
                }

                this.NewWord = string.Empty;
                this.NewMeaning = string.Empty;
                this.NewExample = string.Empty;
                this.NewTranscription = string.Empty;
                this.NewPartOfSpeech = string.Empty;

                this.NavigationRequested?.Invoke($"FlashcardSession:{newBundleId}");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to save bundle '{Title}'.", this.Title);
            }
        }
    }
}