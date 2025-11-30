namespace MasterYourEnglish.Presentation.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using MasterYourEnglish.BLL.DTOs;
    using MasterYourEnglish.BLL.Interfaces;
    using MasterYourEnglish.Presentation.ViewModels.Commands;
    using Microsoft.Extensions.Logging;

    public class TestSessionViewModel : ViewModelBase
    {
        private readonly ITestService testService;
        private readonly ICurrentUserService currentUserService;
        private readonly ILogger<TestSessionViewModel> logger;

        private TestSessionDto currentTest;
        private int currentIndex;
        private Dictionary<int, int> userAnswers = new Dictionary<int, int>();

        // Backing fields
        private string questionText;
        private string progressText;
        private List<TestOptionDto> currentOptions;
        private TestOptionDto selectedOption;

        public TestSessionViewModel(
            ITestService testService,
            ICurrentUserService currentUserService,
            ILogger<TestSessionViewModel> logger)
        {
            this.testService = testService;
            this.currentUserService = currentUserService;
            this.logger = logger;

            this.NextCommand = new RelayCommand(p => this.OnNext());
            this.SubmitCommand = new RelayCommand(p => this.OnSubmit());

            this.logger.LogInformation("TestSessionViewModel initialized.");
        }

        public event Action<string> NavigationRequested;

        public string QuestionText
        {
            get => this.questionText;
            set => this.SetProperty(ref this.questionText, value);
        }

        public string ProgressText
        {
            get => this.progressText;
            set => this.SetProperty(ref this.progressText, value);
        }

        public List<TestOptionDto> CurrentOptions
        {
            get => this.currentOptions;
            set => this.SetProperty(ref this.currentOptions, value);
        }

        public TestOptionDto SelectedOption
        {
            get => this.selectedOption;
            set
            {
                this.SetProperty(ref this.selectedOption, value);
                if (value != null)
                {
                    this.RecordAnswer(value.OptionId);
                }
            }
        }

        public ICommand NextCommand { get; }

        public ICommand SubmitCommand { get; }

        public async void LoadTest(int testId)
        {
            this.logger.LogInformation("Loading test session for test ID: {Id}", testId);
            try
            {
                this.currentTest = await this.testService.GetTestSessionAsync(testId);
                this.currentIndex = 0;
                this.userAnswers.Clear();
                this.LoadCurrentQuestion();
                this.logger.LogInformation("Test session loaded successfully.");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to load test session for ID: {Id}", testId);
            }
        }

        public void LoadTest(TestSessionDto test)
        {
            this.currentTest = test;
            this.logger.LogInformation("Loading test session from generated test. Question Count: {Count}", test.Questions.Count);

            this.currentIndex = 0;
            this.userAnswers.Clear();

            this.LoadCurrentQuestion();
        }

        private void LoadCurrentQuestion()
        {
            if (this.currentTest == null || this.currentTest.Questions.Count == 0)
            {
                this.logger.LogWarning("Attempted to load question, but current test is null or empty.");
                return;
            }

            var q = this.currentTest.Questions[this.currentIndex];
            this.QuestionText = q.Text;
            this.CurrentOptions = q.Options;
            this.SelectedOption = null;
            this.ProgressText = $"Question {this.currentIndex + 1} of {this.currentTest.Questions.Count}";
        }

        private void RecordAnswer(int optionId)
        {
            var qId = this.currentTest.Questions[this.currentIndex].QuestionId;
            this.userAnswers[qId] = optionId;
            this.logger.LogDebug("Recorded answer {OptionId} for question ID {QuestionId}", optionId, qId);
        }

        private void OnNext()
        {
            if (this.currentIndex < this.currentTest.Questions.Count - 1)
            {
                this.currentIndex++;
                this.LoadCurrentQuestion();
            }

            this.logger.LogDebug("Navigating to next question. Index: {Index}", this.currentIndex);
        }

        private async void OnSubmit()
        {
            if (this.currentTest == null)
            {
                this.logger.LogWarning("Submit failed: Current test object is null.");
                return;
            }

            this.logger.LogInformation("Submitting test attempt for test ID: {Id}", this.currentTest.TestId);
            try
            {
                int userId = this.currentUserService.CurrentUser.UserId;
                int score = await this.testService.SubmitTestAttemptAsync(this.currentTest.TestId, userId, this.userAnswers);

                this.logger.LogInformation("Test attempt submitted successfully with score: {Score}", score);
                this.NavigationRequested?.Invoke($"SessionResults:{score}:{this.currentTest.Questions.Count}:Tests");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to submit test attempt for test ID: {Id}", this.currentTest.TestId);
            }
        }
    }
}