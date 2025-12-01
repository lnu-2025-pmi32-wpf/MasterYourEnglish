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
    using Microsoft.Extensions.Logging;

    public class CreateTestQuestionViewModel : ViewModelBase
    {
        public string Text { get; set; }

        public string CorrectAnswer { get; set; }

        public string Option1 { get; set; }

        public string Option2 { get; set; }

        public string Option3 { get; set; }

        public string Option4 { get; set; }

        public int CorrectOptionIndex { get; set; }
    }

    public class CreateTestViewModel : ViewModelBase, IPageViewModel
    {
        private readonly ITopicService topicService;
        private readonly ITestService testService;
        private readonly ICurrentUserService currentUserService;
        private readonly ILogger<CreateTestViewModel> logger;

        public event Action<string> NavigationRequested;

        private string title = "New Test";

        public string Title { get => this.title; set => this.SetProperty(ref this.title, value); }

        private string description = string.Empty;

        public string Description { get => this.description; set => this.SetProperty(ref this.description, value); }

        private TopicDto selectedTopic;

        public TopicDto SelectedTopic
        {
            get => this.selectedTopic;
            set => this.SetProperty(ref this.selectedTopic, value);
        }

        public ObservableCollection<TopicDto> AvailableTopics { get; } = new ObservableCollection<TopicDto>();

        private string difficulty = "B2";

        public string Difficulty { get => this.difficulty; set => this.SetProperty(ref this.difficulty, value); }

        private string newQuestionText;

        public string NewQuestionText { get => this.newQuestionText; set => this.SetProperty(ref this.newQuestionText, value); }

        private string option1;

        public string Option1 { get => this.option1; set => this.SetProperty(ref this.option1, value); }

        private string option2;

        public string Option2 { get => this.option2; set => this.SetProperty(ref this.option2, value); }

        private string option3;

        public string Option3 { get => this.option3; set => this.SetProperty(ref this.option3, value); }

        private string option4;

        public string Option4 { get => this.option4; set => this.SetProperty(ref this.option4, value); }

        private bool isOption1Correct = true;

        public bool IsOption1Correct { get => this.isOption1Correct; set => this.SetProperty(ref this.isOption1Correct, value); }

        private bool isOption2Correct;

        public bool IsOption2Correct { get => this.isOption2Correct; set => this.SetProperty(ref this.isOption2Correct, value); }

        private bool isOption3Correct;

        public bool IsOption3Correct { get => this.isOption3Correct; set => this.SetProperty(ref this.isOption3Correct, value); }

        private bool isOption4Correct;

        public bool IsOption4Correct { get => this.isOption4Correct; set => this.SetProperty(ref this.isOption4Correct, value); }

        public ObservableCollection<CreateTestQuestionViewModel> QuestionsList { get; } = new ObservableCollection<CreateTestQuestionViewModel>();

        public ICommand AddQuestionCommand { get; }

        public ICommand SaveTestCommand { get; }

        public ICommand CancelCommand { get; }

        public CreateTestViewModel(
            ITopicService topicService,
            ITestService testService,
            ICurrentUserService currentUserService,
            ILogger<CreateTestViewModel> logger)
        {
            this.topicService = topicService;
            this.testService = testService;
            this.currentUserService = currentUserService;
            this.logger = logger;

            this.AddQuestionCommand = new RelayCommand(p => this.OnAddQuestion());
            this.SaveTestCommand = new RelayCommand(p => this.OnSaveTest());
            this.CancelCommand = new RelayCommand(p => this.NavigationRequested?.Invoke("Tests"));
            this.logger.LogInformation("Create Test view model initialized.");
        }

        public async Task LoadDataAsync()
        {
            this.logger.LogInformation("Starting to load topics for test creation.");
            try
            {
                var topics = await this.topicService.GetAllTopicsAsync();
                this.AvailableTopics.Clear();
                foreach (var t in topics)
                {
                    this.AvailableTopics.Add(t);
                }

                this.SelectedTopic = this.AvailableTopics.FirstOrDefault();
                this.logger.LogInformation("Loaded {Count} topics successfully.", topics.Count);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to load topics for test creation.");
            }
        }

        private void OnAddQuestion()
        {
            int correctIndex = 0;
            string correctText = this.Option1;

            if (this.IsOption2Correct) { correctIndex = 1;
                correctText = this.Option2; }
            else if (this.IsOption3Correct) { correctIndex = 2;
                correctText = this.Option3; }
            else if (this.IsOption4Correct) { correctIndex = 3;
                correctText = this.Option4; }

            if (string.IsNullOrWhiteSpace(this.NewQuestionText) ||
                string.IsNullOrWhiteSpace(this.Option1) ||
                string.IsNullOrWhiteSpace(this.Option2))
            {
                this.logger.LogWarning("Cannot add question: Question text or required options (1/2) are missing.");
                return;
            }

            this.QuestionsList.Add(new CreateTestQuestionViewModel
            {
                Text = this.NewQuestionText,
                CorrectAnswer = correctText,

                Option1 = this.Option1,
                Option2 = this.Option2,
                Option3 = this.Option3,
                Option4 = this.Option4,
                CorrectOptionIndex = correctIndex,
            });

            this.logger.LogDebug("Question added: {Text}", this.NewQuestionText);

            this.NewQuestionText = string.Empty;
            this.Option1 = string.Empty;
            this.Option2 = string.Empty;
            this.Option3 = string.Empty;
            this.Option4 = string.Empty;
            this.IsOption1Correct = true;
            this.IsOption2Correct = false;
            this.IsOption3Correct = false;
            this.IsOption4Correct = false;
        }

        private async void OnSaveTest()
        {
            if (this.SelectedTopic == null || this.QuestionsList.Count == 0)
            {
                this.logger.LogWarning("Save failed: Selected topic is null or no questions are added.");
                return;
            }

            this.logger.LogInformation("Attempting to save new test: {Title}", this.Title);

            try
            {
                int userId = this.currentUserService.CurrentUser.UserId;

                var dto = new CreateTestDto
                {
                    Title = this.Title,
                    Description = this.Description,
                    TopicId = this.SelectedTopic.TopicId,
                    DifficultyLevel = this.Difficulty,

                    NewQuestions = this.QuestionsList.Select(q => new CreateQuestionDto
                    {
                        Text = q.Text,
                        Options = new List<CreateOptionDto>
                        {
                            new CreateOptionDto { Text = q.Option1 },
                            new CreateOptionDto { Text = q.Option2 },
                            new CreateOptionDto { Text = q.Option3 },
                            new CreateOptionDto { Text = q.Option4 },
                        },
                        CorrectOptionIndex = q.CorrectOptionIndex,
                    }).ToList(),
                };

                int newId = await this.testService.CreateNewTestAsync(dto, userId);

                this.logger.LogInformation("Test '{Title}' created successfully with ID: {Id}", this.Title, newId);

                this.Title = "New Test";
                this.Description = string.Empty;
                this.Difficulty = "B2";
                this.QuestionsList.Clear();
                if (this.AvailableTopics.Count > 0)
                {
                    this.SelectedTopic = this.AvailableTopics.FirstOrDefault();
                }

                this.NewQuestionText = string.Empty;
                this.Option1 = string.Empty;
                this.Option2 = string.Empty;
                this.Option3 = string.Empty;
                this.Option4 = string.Empty;
                this.IsOption1Correct = true;
                this.IsOption2Correct = false;
                this.IsOption3Correct = false;
                this.IsOption4Correct = false;

                this.NavigationRequested?.Invoke("Tests");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to save test '{Title}'.", this.Title);
            }
        }
    }
}