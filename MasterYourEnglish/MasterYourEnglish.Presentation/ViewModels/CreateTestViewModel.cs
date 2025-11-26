using MasterYourEnglish.BLL.DTOs;
using MasterYourEnglish.BLL.Interfaces;
using MasterYourEnglish.BLL.Models.DTOs;
using MasterYourEnglish.Presentation.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MasterYourEnglish.Presentation.ViewModels
{
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
        private readonly ITopicService _topicService;
        private readonly ITestService _testService;
        private readonly ICurrentUserService _currentUserService;
        public event Action<string> NavigationRequested;

        public string Title { get; set; } = "New Test";
        public string Description { get; set; } = "";

        private TopicDto _selectedTopic;
        public TopicDto SelectedTopic
        {
            get => _selectedTopic;
            set => SetProperty(ref _selectedTopic, value);
        }

        public ObservableCollection<TopicDto> AvailableTopics { get; } = new ObservableCollection<TopicDto>();
        public string Difficulty { get; set; } = "B2";

        private string _newQuestionText;
        public string NewQuestionText { get => _newQuestionText; set => SetProperty(ref _newQuestionText, value); }

        private string _option1;
        public string Option1 { get => _option1; set => SetProperty(ref _option1, value); }

        private string _option2;
        public string Option2 { get => _option2; set => SetProperty(ref _option2, value); }

        private string _option3;
        public string Option3 { get => _option3; set => SetProperty(ref _option3, value); }

        private string _option4;
        public string Option4 { get => _option4; set => SetProperty(ref _option4, value); }

        private bool _isOption1Correct = true;
        public bool IsOption1Correct { get => _isOption1Correct; set => SetProperty(ref _isOption1Correct, value); }

        private bool _isOption2Correct;
        public bool IsOption2Correct { get => _isOption2Correct; set => SetProperty(ref _isOption2Correct, value); }

        private bool _isOption3Correct;
        public bool IsOption3Correct { get => _isOption3Correct; set => SetProperty(ref _isOption3Correct, value); }

        private bool _isOption4Correct;
        public bool IsOption4Correct { get => _isOption4Correct; set => SetProperty(ref _isOption4Correct, value); }

        public ObservableCollection<CreateTestQuestionViewModel> QuestionsList { get; } = new ObservableCollection<CreateTestQuestionViewModel>();

        public ICommand AddQuestionCommand { get; }
        public ICommand SaveTestCommand { get; }
        public ICommand CancelCommand { get; }

        public CreateTestViewModel(ITopicService topicService, ITestService testService, ICurrentUserService currentUserService)
        {
            _topicService = topicService;
            _testService = testService;
            _currentUserService = currentUserService;

            AddQuestionCommand = new RelayCommand(p => OnAddQuestion());
            SaveTestCommand = new RelayCommand(p => OnSaveTest());
            CancelCommand = new RelayCommand(p => NavigationRequested?.Invoke("Tests"));
        }

        public async Task LoadDataAsync()
        {
            var topics = await _topicService.GetAllTopicsAsync();
            AvailableTopics.Clear();
            foreach (var t in topics) AvailableTopics.Add(t);
            SelectedTopic = AvailableTopics.FirstOrDefault();
        }

        private void OnAddQuestion()
        {
            int correctIndex = 0;
            string correctText = Option1;

            if (IsOption2Correct) { correctIndex = 1; correctText = Option2; }
            else if (IsOption3Correct) { correctIndex = 2; correctText = Option3; }
            else if (IsOption4Correct) { correctIndex = 3; correctText = Option4; }

            if (string.IsNullOrWhiteSpace(NewQuestionText) ||
                string.IsNullOrWhiteSpace(Option1) ||
                string.IsNullOrWhiteSpace(Option2))
            {
                return;
            }

            QuestionsList.Add(new CreateTestQuestionViewModel
            {
                Text = NewQuestionText,
                CorrectAnswer = correctText,

                Option1 = this.Option1,
                Option2 = this.Option2,
                Option3 = this.Option3,
                Option4 = this.Option4,
                CorrectOptionIndex = correctIndex
            });

            NewQuestionText = "";
            Option1 = ""; Option2 = ""; Option3 = ""; Option4 = "";
            IsOption1Correct = true; IsOption2Correct = false; IsOption3Correct = false; IsOption4Correct = false;
        }

        private async void OnSaveTest()
        {
            if (SelectedTopic == null || QuestionsList.Count == 0) return;

            int userId = _currentUserService.CurrentUser.UserId;

            var dto = new CreateTestDto
            {
                Title = Title,
                Description = Description,
                TopicId = SelectedTopic.TopicId,
                DifficultyLevel = Difficulty,

                NewQuestions = QuestionsList.Select(q => new CreateQuestionDto
                {
                    Text = q.Text,
                    Options = new List<CreateOptionDto>
                    {
                        new CreateOptionDto { Text = q.Option1 },
                        new CreateOptionDto { Text = q.Option2 },
                        new CreateOptionDto { Text = q.Option3 },
                        new CreateOptionDto { Text = q.Option4 }
                    },
                    CorrectOptionIndex = q.CorrectOptionIndex
                }).ToList()
            };

            int newId = await _testService.CreateNewTestAsync(dto, userId);

            Title = "New Test";
            Description = "";
            Difficulty = "B2";
            QuestionsList.Clear();
            if (AvailableTopics.Count > 0) SelectedTopic = AvailableTopics[0];

            NewQuestionText = "";
            Option1 = ""; Option2 = ""; Option3 = ""; Option4 = "";
            IsOption1Correct = true; IsOption2Correct = false; IsOption3Correct = false; IsOption4Correct = false;

            NavigationRequested?.Invoke("Tests");
        }
    }
}