namespace MasterYourEnglish.Presentation.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using MasterYourEnglish.BLL.DTOs;
    using MasterYourEnglish.BLL.Interfaces;
    using MasterYourEnglish.Presentation.ViewModels.Commands;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class GenerateBundleViewModel : ViewModelBase, IPageViewModel
    {
        private readonly ITopicService topicService;
        private readonly IFlashcardBundleService bundleService;
        private readonly ICurrentUserService currentUserService;
        private readonly ILogger<GenerateBundleViewModel> logger;
        private readonly IServiceProvider serviceProvider;
        private string selectedMinLevel;
        private string selectedMaxLevel;

        public GenerateBundleViewModel(
            ITopicService topicService,
            IFlashcardBundleService bundleService,
            ICurrentUserService currentUserService,
            ILogger<GenerateBundleViewModel> logger,
            IServiceProvider serviceProvider)
        {
            this.topicService = topicService;
            this.bundleService = bundleService;
            this.currentUserService = currentUserService;
            this.logger = logger;
            this.serviceProvider = serviceProvider;

            this.Topics = new ObservableCollection<TopicSelectionViewModel>();
            this.Levels = new List<string> { "A1", "A2", "B1", "B2", "C1" };
            this.selectedMinLevel = this.Levels.First();
            this.selectedMaxLevel = this.Levels.Last();

            this.GenerateCommand = new RelayCommand(async p => await this.OnGenerate());
            this.CancelCommand = new RelayCommand(p => this.NavigationRequested?.Invoke("Flashcards"));

            this.logger.LogInformation("GenerateBundleViewModel initialized.");
        }

        public event Action<List<FlashcardSessionDto>> SessionGenerated;

        public event Action<string> NavigationRequested;

        public ObservableCollection<TopicSelectionViewModel> Topics { get; }

        public List<string> Levels { get; }

        public string SelectedMinLevel
        {
            get => this.selectedMinLevel;
            set => this.SetProperty(ref this.selectedMinLevel, value);
        }

        public string SelectedMaxLevel
        {
            get => this.selectedMaxLevel;
            set => this.SetProperty(ref this.selectedMaxLevel, value);
        }

        public ICommand GenerateCommand { get; }

        public ICommand CancelCommand { get; }

        public async Task LoadDataAsync()
        {
            this.logger.LogInformation("Loading available topics for bundle generation.");
            try
            {
                var topicDtos = await this.topicService.GetAllTopicsAsync();
                this.Topics.Clear();
                foreach (var dto in topicDtos)
                {
                    var topicLogger = this.serviceProvider.GetRequiredService<ILogger<TopicSelectionViewModel>>();
                    this.Topics.Add(new TopicSelectionViewModel(dto, topicLogger));
                }

                this.logger.LogInformation("Successfully loaded {Count} topics.", topicDtos.Count);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to load topics for bundle generation.");
            }
        }

        private async Task OnGenerate()
        {
            this.logger.LogInformation("Starting flashcard session generation based on user criteria.");

            int minIndex = this.Levels.IndexOf(this.selectedMinLevel);
            int maxIndex = this.Levels.IndexOf(this.selectedMaxLevel);

            if (minIndex > maxIndex)
            {
                this.logger.LogWarning("Generation stopped: Minimum level ({Min}) is higher than maximum level ({Max}).", this.selectedMinLevel, this.selectedMaxLevel);
                return;
            }

            var selectedLevels = this.Levels.Skip(minIndex).Take(maxIndex - minIndex + 1).ToList();
            var topicRequests = this.Topics
                .Where(t => t.CardCount > 0)
                .ToDictionary(t => t.TopicId, t => t.CardCount);

            if (topicRequests.Count == 0)
            {
                this.logger.LogWarning("Generation failed: No topics selected or card count is zero.");
                return;
            }

            try
            {
                int userId = this.currentUserService.CurrentUser.UserId;
                this.logger.LogDebug(
                    "Generating session for levels: {Levels}, Topics: {TopicCount}",
                    string.Join(",", selectedLevels), topicRequests.Count);

                List<FlashcardSessionDto> generatedCards = await this.bundleService.GetGeneratedSessionAsync(userId, selectedLevels, topicRequests);

                if (generatedCards.Any())
                {
                    this.logger.LogInformation("Successfully generated {Count} cards.", generatedCards.Count);
                    this.SessionGenerated?.Invoke(generatedCards);
                }
                else
                {
                    this.logger.LogWarning("Generated session is empty. No cards found based on criteria.");
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Exception occurred during session generation.");
            }
        }
    }
}