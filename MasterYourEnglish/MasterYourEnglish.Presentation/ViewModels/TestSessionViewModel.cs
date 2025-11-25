using MasterYourEnglish.BLL.DTOs;
using MasterYourEnglish.BLL.Interfaces;
using MasterYourEnglish.Presentation.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace MasterYourEnglish.Presentation.ViewModels
{
    public class TestSessionViewModel : ViewModelBase
    {
        private readonly ITestService _testService;
        private readonly ICurrentUserService _currentUserService;

        private TestSessionDto _currentTest;
        private int _currentIndex;
        private Dictionary<int, int> _userAnswers = new Dictionary<int, int>();

        public event Action<string> NavigationRequested;

        private string _questionText;
        public string QuestionText { get => _questionText; set => SetProperty(ref _questionText, value); }

        private string _progressText;
        public string ProgressText { get => _progressText; set => SetProperty(ref _progressText, value); }

        private List<TestOptionDto> _currentOptions;
        public List<TestOptionDto> CurrentOptions { get => _currentOptions; set => SetProperty(ref _currentOptions, value); }

        private TestOptionDto _selectedOption;
        public TestOptionDto SelectedOption
        {
            get => _selectedOption;
            set
            {
                SetProperty(ref _selectedOption, value);
                if (value != null) RecordAnswer(value.OptionId);
            }
        }

        public ICommand NextCommand { get; }
        public ICommand SubmitCommand { get; }

        public TestSessionViewModel(ITestService testService, ICurrentUserService currentUserService)
        {
            _testService = testService;
            _currentUserService = currentUserService;
            NextCommand = new RelayCommand(p => OnNext());
            SubmitCommand = new RelayCommand(p => OnSubmit());
        }

        public async void LoadTest(int testId)
        {
            _currentTest = await _testService.GetTestSessionAsync(testId);
            _currentIndex = 0;
            _userAnswers.Clear();
            LoadCurrentQuestion();
        }

        private void LoadCurrentQuestion()
        {
            if (_currentTest == null || _currentTest.Questions.Count == 0) return;

            var q = _currentTest.Questions[_currentIndex];
            QuestionText = q.Text;
            CurrentOptions = q.Options;
            SelectedOption = null;
            ProgressText = $"Question {_currentIndex + 1} of {_currentTest.Questions.Count}";
        }

        private void RecordAnswer(int optionId)
        {
            var qId = _currentTest.Questions[_currentIndex].QuestionId;
            _userAnswers[qId] = optionId;
        }

        private void OnNext()
        {
            if (_currentIndex < _currentTest.Questions.Count - 1)
            {
                _currentIndex++;
                LoadCurrentQuestion();
            }
        }

        private async void OnSubmit()
        {
            int userId = _currentUserService.CurrentUser.UserId;
            int score = await _testService.SubmitTestAttemptAsync(_currentTest.TestId, userId, _userAnswers);

            NavigationRequested?.Invoke($"SessionResults:{score}:{_currentTest.Questions.Count}:Tests");
        }
    }
}