namespace MasterYourEnglish.Presentation.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
    using MasterYourEnglish.BLL.DTOs;
    using MasterYourEnglish.BLL.Interfaces;
    using MasterYourEnglish.Presentation.ViewModels.Commands;
    using Microsoft.Extensions.Logging;

    public class SelectableOptionViewModel : ViewModelBase
    {
        private bool isSelected;

        public int OptionId { get; set; }

        public string Text { get; set; }

        public bool IsSelected
        {
            get => this.isSelected;
            set => this.SetProperty(ref this.isSelected, value);
        }
    }

    /// <summary>
    /// Represents a matching pair for matching questions.
    /// The user sees LeftText and selects from AvailableMatches to find the correct pair.
    /// </summary>
    public class MatchingPairViewModel : ViewModelBase
    {
        private string selectedMatch;

        public int OptionId { get; set; }

        public string LeftText { get; set; }

        public string CorrectMatch { get; set; }

        public List<string> AvailableMatches { get; set; } = new List<string>();

        public string SelectedMatch
        {
            get => this.selectedMatch;
            set => this.SetProperty(ref this.selectedMatch, value);
        }

        public bool IsCorrect => this.SelectedMatch == this.CorrectMatch;
    }

    public class TestSessionViewModel : ViewModelBase
    {
        private readonly ITestService testService;
        private readonly ICurrentUserService currentUserService;
        private readonly ILogger<TestSessionViewModel> logger;

        private TestSessionDto currentTest;
        private int currentIndex;
        private Dictionary<int, List<int>> userAnswers = new Dictionary<int, List<int>>();

        // Backing fields
        private string questionText;
        private string progressText;
        private string currentQuestionType = "SingleChoice";
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

        private string sessionTitle = "Session";

        public string SessionTitle
        {
            get => this.sessionTitle;
            set => this.SetProperty(ref this.sessionTitle, value);
        }

        public string CurrentQuestionType
        {
            get => this.currentQuestionType;
            set
            {
                if (this.SetProperty(ref this.currentQuestionType, value))
                {
                    this.OnPropertyChanged(nameof(this.DisplayQuestionType));
                }
            }
        }

        public string DisplayQuestionType
        {
            get
            {
                return this.CurrentQuestionType switch
                {
                    "SingleChoice" => "Single Choice",
                    "MultipleChoice" => "Multiple Choice",
                    _ => this.CurrentQuestionType ?? "Unknown"
                };
            }
        }

        public bool IsSingleChoice => this.CurrentQuestionType == "SingleChoice";

        public bool IsMultipleChoice => this.CurrentQuestionType == "MultipleChoice";

        public bool IsMatching => this.CurrentQuestionType == "Matching";

        public List<TestOptionDto> CurrentOptions
        {
            get => this.currentOptions;
            set => this.SetProperty(ref this.currentOptions, value);
        }

        public ObservableCollection<SelectableOptionViewModel> SelectableOptions { get; } = new ObservableCollection<SelectableOptionViewModel>();

        public ObservableCollection<MatchingPairViewModel> MatchingPairs { get; } = new ObservableCollection<MatchingPairViewModel>();

        public TestOptionDto SelectedOption
        {
            get => this.selectedOption;
            set
            {
                this.SetProperty(ref this.selectedOption, value);
                if (value != null && this.IsSingleChoice)
                {
                    this.RecordSingleAnswer(value.OptionId);
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
                
                if (this.currentTest != null)
                {
                    this.SessionTitle = this.currentTest.Title;
                }
                else
                {
                    this.SessionTitle = "Session";
                }

                this.currentIndex = 0;
                this.userAnswers.Clear();
                this.LoadCurrentQuestion();
                this.logger.LogInformation("Test session loaded successfully.");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to load test session for ID: {Id}", testId);
                this.SessionTitle = "Session";
            }
        }

        public void LoadTest(TestSessionDto test)
        {
            this.currentTest = test;
            this.SessionTitle = test.Title;
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
            this.CurrentQuestionType = q.QuestionType ?? "SingleChoice";
            this.CurrentOptions = q.Options;
            this.SelectableOptions.Clear();
            foreach (var opt in q.Options)
            {
                this.SelectableOptions.Add(new SelectableOptionViewModel
                {
                    OptionId = opt.OptionId,
                    Text = opt.Text,
                    IsSelected = false,
                });
            }
            this.MatchingPairs.Clear();
            if (this.IsMatching)
            {
                var allMatches = q.Options
                    .Where(o => !string.IsNullOrWhiteSpace(o.MatchingText))
                    .Select(o => o.MatchingText)
                    .OrderBy(x => Guid.NewGuid())
                    .ToList();

                foreach (var opt in q.Options)
                {
                    if (!string.IsNullOrWhiteSpace(opt.MatchingText))
                    {
                        this.MatchingPairs.Add(new MatchingPairViewModel
                        {
                            OptionId = opt.OptionId,
                            LeftText = opt.Text,
                            CorrectMatch = opt.MatchingText,
                            AvailableMatches = allMatches.ToList(),
                            SelectedMatch = null,
                        });
                    }
                }
            }

            this.SelectedOption = null;
            this.ProgressText = $"Question {this.currentIndex + 1} of {this.currentTest.Questions.Count}";

            this.OnPropertyChanged(nameof(this.IsSingleChoice));
            this.OnPropertyChanged(nameof(this.IsMultipleChoice));
            this.OnPropertyChanged(nameof(this.IsMatching));
        }

        private void RecordSingleAnswer(int optionId)
        {
            var qId = this.currentTest.Questions[this.currentIndex].QuestionId;
            this.userAnswers[qId] = new List<int> { optionId };
            this.logger.LogDebug("Recorded single answer {OptionId} for question ID {QuestionId}", optionId, qId);
        }

        public void RecordMultipleAnswers()
        {
            var qId = this.currentTest.Questions[this.currentIndex].QuestionId;
            var selectedIds = this.SelectableOptions.Where(o => o.IsSelected).Select(o => o.OptionId).ToList();
            this.userAnswers[qId] = selectedIds;
            this.logger.LogDebug("Recorded {Count} answers for question ID {QuestionId}", selectedIds.Count, qId);
        }

        public void RecordMatchingAnswers()
        {
            var qId = this.currentTest.Questions[this.currentIndex].QuestionId;
            bool allCorrect = this.MatchingPairs.All(p => p.IsCorrect);

            if (allCorrect && this.MatchingPairs.Count > 0)
            {
                this.userAnswers[qId] = new List<int> { this.MatchingPairs.First().OptionId };
                this.logger.LogDebug("All matching pairs correct for question ID {QuestionId}", qId);
            }
            else
            {
                this.userAnswers[qId] = new List<int> { -1 };
                this.logger.LogDebug("Matching pairs incorrect for question ID {QuestionId}. Correct: {Correct}/{Total}", 
                    qId, this.MatchingPairs.Count(p => p.IsCorrect), this.MatchingPairs.Count);
            }
        }

        private void OnNext()
        {
            if (this.IsMultipleChoice)
            {
                this.RecordMultipleAnswers();
            }
            else if (this.IsMatching)
            {
                this.RecordMatchingAnswers();
            }

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

            if (this.IsMultipleChoice)
            {
                this.RecordMultipleAnswers();
            }
            else if (this.IsMatching)
            {
                this.RecordMatchingAnswers();
            }

            this.logger.LogInformation("Submitting test attempt for test ID: {Id}", this.currentTest.TestId);
            try
            {
                int userId = this.currentUserService.CurrentUser.UserId;

                var singleAnswers = this.userAnswers.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.FirstOrDefault());

                int score = await this.testService.SubmitTestAttemptAsync(this.currentTest.TestId, userId, singleAnswers);

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