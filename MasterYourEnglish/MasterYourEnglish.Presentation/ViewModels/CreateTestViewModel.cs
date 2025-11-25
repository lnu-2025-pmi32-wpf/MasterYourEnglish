using MasterYourEnglish.BLL.DTOs;
using MasterYourEnglish.BLL.Interfaces;
using MasterYourEnglish.BLL.Models.DTOs;
using MasterYourEnglish.Presentation.ViewModels.Commands;
using System;
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

    }

    public class CreateTestViewModel : ViewModelBase, IPageViewModel
    {
        private readonly ITopicService _topicService;
        private readonly ITestService _testService;
        private readonly ICurrentUserService _currentUserService;
        public event Action<string> NavigationRequested;

        public string Title { get; set; } = "New Test";
        public string Description { get; set; } = "";
        public TopicDto SelectedTopic { get; set; }
        public ObservableCollection<TopicDto> AvailableTopics { get; } = new ObservableCollection<TopicDto>();
        public string Difficulty { get; set; } = "B2";

        public string NewQuestionText { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string Option4 { get; set; }

        public bool IsOption1Correct { get; set; } = true; // Default
        public bool IsOption2Correct { get; set; }
        public bool IsOption3Correct { get; set; }
        public bool IsOption4Correct { get; set; }

        public int CorrectOptionIndex { get; set; } = 0;

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
            if (IsOption2Correct) correctIndex = 1;
            if (IsOption3Correct) correctIndex = 2;
            if (IsOption4Correct) correctIndex = 3;

            QuestionsList.Add(new CreateTestQuestionViewModel
            {
                Text = NewQuestionText,
                CorrectAnswer = Option1
            });

            NewQuestionText = "";
            Option1 = ""; Option2 = ""; Option3 = ""; Option4 = "";
            IsOption1Correct = true; IsOption2Correct = false;
        }

        private async void OnSaveTest()
        {
            var dto = new CreateTestDto
            {
                Title = Title,
                Description = Description,
                TopicId = SelectedTopic.TopicId,
                DifficultyLevel = Difficulty,
                NewQuestions = QuestionsList.Select(q => new CreateQuestionDto
                {
                    Text = q.Text,
                    Options = new System.Collections.Generic.List<CreateOptionDto>
                    {
                        new CreateOptionDto { Text = "Option A" },
                        new CreateOptionDto { Text = "Option B" }
                    },
                    CorrectOptionIndex = 0
                }).ToList()
            };

            int newId = await _testService.CreateNewTestAsync(dto, _currentUserService.CurrentUser.UserId);
            NavigationRequested?.Invoke("Tests");
        }
    }
}