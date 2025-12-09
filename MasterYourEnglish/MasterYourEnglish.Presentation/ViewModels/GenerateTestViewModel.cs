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
    using MasterYourEnglish.BLL.Models.DTOs;
    using MasterYourEnglish.Presentation.ViewModels.Commands;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class GenerateTestViewModel : ViewModelBase, IPageViewModel
    {
        private readonly ITopicService topicService;
        private readonly ITestService testService;
        private readonly ICurrentUserService currentUserService;
        private readonly ILogger<GenerateTestViewModel> logger;
        private readonly IServiceProvider serviceProvider;
        private string selectedMinLevel;
        private string selectedMaxLevel;

        public GenerateTestViewModel(
            ITopicService topicService,
            ITestService testService,
            ICurrentUserService currentUserService,
            ILogger<GenerateTestViewModel> logger,
            IServiceProvider serviceProvider)
        {
            this.topicService = topicService;
            this.testService = testService;
            this.currentUserService = currentUserService;
            this.logger = logger;
            this.serviceProvider = serviceProvider;

            this.Topics = new ObservableCollection<TestTopicSelectionViewModel>();
            this.Levels = new List<string> { "A1", "A2", "B1", "B2", "C1", "C2" };
            this.selectedMinLevel = this.Levels.First();
            this.selectedMaxLevel = this.Levels.Last();

            this.GenerateCommand = new RelayCommand(async p => await this.OnGenerate());
            this.CancelCommand = new RelayCommand(p => this.NavigationRequested?.Invoke("Tests"));

            this.logger.LogInformation("GenerateTestViewModel initialized.");
        }

        public event Action<List<TestSessionDto>> TestSessionGenerated;

        public event Action<string> NavigationRequested;

        public ObservableCollection<TestTopicSelectionViewModel> Topics { get; }

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
            this.logger.LogInformation("Starting to load topics for test generation criteria.");
            try
            {
                var topicDtos = await this.topicService.GetAllTopicsAsync();
                this.Topics.Clear();
                foreach (var dto in topicDtos)
                {
                    this.Topics.Add(new TestTopicSelectionViewModel(dto));
                }

                this.logger.LogInformation("Loaded {Count} topics successfully.", topicDtos.Count);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to load topics for test generation.");
            }
        }

        private async Task OnGenerate()
        {
            this.logger.LogInformation("Starting test session generation based on user criteria.");

            int minIndex = this.Levels.IndexOf(this.selectedMinLevel);
            int maxIndex = this.Levels.IndexOf(this.selectedMaxLevel);

            if (minIndex > maxIndex)
            {
                this.logger.LogWarning(
                    "Generation stopped: Minimum level ({Min}) is higher than maximum level ({Max}).",
                    this.selectedMinLevel,
                    this.selectedMaxLevel);
                return;
            }

            var selectedLevels = this.Levels.Skip(minIndex).Take(maxIndex - minIndex + 1).ToList();

            var requests = this.Topics
                .Where(t => t.TotalCount > 0)
                .Select(t => new TestGenerationRequest
                {
                    TopicId = t.TopicId,
                    SingleChoiceCount = t.SingleChoiceCount,
                    MultipleChoiceCount = t.MultipleChoiceCount,
                    MatchingCount = t.MatchingCount
                })
                .ToList();

            if (requests.Count == 0)
            {
                this.logger.LogWarning("Generation failed: No topics selected or question count is zero.");
                return;
            }

            this.logger.LogInformation(
                "Generating test session with levels {Min} to {Max} and {TopicCount} topics.",
                this.selectedMinLevel,
                this.selectedMaxLevel,
                requests.Count);

            try
            {
                int userId = this.currentUserService.CurrentUser.UserId;

                List<TestSessionDto> generatedSession = await this.testService.GetGeneratedTestSessionAsync(userId, selectedLevels, requests);

                if (generatedSession != null && generatedSession.Any())
                {
                    this.logger.LogInformation("Successfully generated {Count} questions.", generatedSession.Count);
                    this.TestSessionGenerated?.Invoke(generatedSession);
                }
                else
                {
                    this.logger.LogWarning("Generated session is null or empty. No questions found based on criteria.");
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to generate test session.");
            }
        }
    }

    public class TestTopicSelectionViewModel : ViewModelBase
    {
        private int singleChoiceCount;
        private int multipleChoiceCount;
        private int matchingCount;

        public TestTopicSelectionViewModel(TopicDto topic)
        {
            this.TopicId = topic.TopicId;
            this.Name = topic.Name;
        }

        public int TopicId { get; }
        public string Name { get; }

        public int SingleChoiceCount
        {
            get => this.singleChoiceCount;
            set { this.SetProperty(ref this.singleChoiceCount, value); this.OnPropertyChanged(nameof(this.TotalCount)); }
        }

        public int MultipleChoiceCount
        {
            get => this.multipleChoiceCount;
            set { this.SetProperty(ref this.multipleChoiceCount, value); this.OnPropertyChanged(nameof(this.TotalCount)); }
        }

        public int MatchingCount
        {
            get => this.matchingCount;
            set { this.SetProperty(ref this.matchingCount, value); this.OnPropertyChanged(nameof(this.TotalCount)); }
        }

        public int TotalCount => this.SingleChoiceCount + this.MultipleChoiceCount + this.MatchingCount;
    }
}