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
    public class CreateBundleViewModel : ViewModelBase, IPageViewModel
    {
        private readonly ITopicService _topicService;
        private readonly IFlashcardBundleService _bundleService;
        private readonly ICurrentUserService _currentUserService;
        public event Action<string> NavigationRequested;

        private string _title = "My New Bundle";
        public string Title { get => _title; set => SetProperty(ref _title, value); }

        private string _description = "";
        public string Description { get => _description; set => SetProperty(ref _description, value); }

        private TopicDto _selectedTopic;
        public TopicDto SelectedTopic { get => _selectedTopic; set => SetProperty(ref _selectedTopic, value); }

        public ObservableCollection<TopicDto> AvailableTopics { get; }

        private string _difficulty = "B2";
        public string Difficulty { get => _difficulty; set => SetProperty(ref _difficulty, value); }

        private string _newWord;
        public string NewWord { get => _newWord; set => SetProperty(ref _newWord, value); }

        private string _newMeaning;
        public string NewMeaning { get => _newMeaning; set => SetProperty(ref _newMeaning, value); }

        private string _newExample;
        public string NewExample { get => _newExample; set => SetProperty(ref _newExample, value); }

        private string _newTranscription;
        public string NewTranscription { get => _newTranscription; set => SetProperty(ref _newTranscription, value); }

        private string _newPartOfSpeech;
        public string NewPartOfSpeech { get => _newPartOfSpeech; set => SetProperty(ref _newPartOfSpeech, value); }

        public ObservableCollection<CreateFlashcardDto> NewFlashcards { get; }

        public ICommand AddCardCommand { get; }
        public ICommand RemoveCardCommand { get; }
        public ICommand SaveBundleCommand { get; }
        public ICommand CancelCommand { get; }

        public CreateBundleViewModel(ITopicService topicService, IFlashcardBundleService bundleService, ICurrentUserService currentUserService)
        {
            _topicService = topicService;
            _bundleService = bundleService;
            _currentUserService = currentUserService;

            NewFlashcards = new ObservableCollection<CreateFlashcardDto>();
            AvailableTopics = new ObservableCollection<TopicDto>();

            AddCardCommand = new RelayCommand(p => OnAddCard(), p => CanAddCard());
            RemoveCardCommand = new RelayCommand(OnRemoveCard);
            SaveBundleCommand = new RelayCommand(async p => await OnSaveBundle(), p => CanSaveBundle());
            CancelCommand = new RelayCommand(p => NavigationRequested?.Invoke("Flashcards"));
        }

        public async Task LoadDataAsync()
        {
            var topicDtos = await _topicService.GetAllTopicsAsync();
            AvailableTopics.Clear();
            foreach (var dto in topicDtos)
            {
                AvailableTopics.Add(dto);
            }
            SelectedTopic = AvailableTopics.FirstOrDefault();
        }

        private bool CanAddCard() => !string.IsNullOrWhiteSpace(NewWord) && !string.IsNullOrWhiteSpace(NewMeaning);
        private void OnAddCard()
        {
            var newCard = new CreateFlashcardDto
            {
                Word = this.NewWord,
                Meaning = this.NewMeaning,
                Example = this.NewExample ?? "",
                DifficultyLevel = this.Difficulty,

                Transcription = this.NewTranscription ?? "",
                PartOfSpeech = this.NewPartOfSpeech ?? ""
            };

            NewFlashcards.Add(newCard);

            NewWord = "";
            NewMeaning = "";
            NewExample = "";

            NewTranscription = "";
            NewPartOfSpeech = "";
        }

        private void OnRemoveCard(object parameter)
        {
            if (parameter is CreateFlashcardDto card)
            {
                NewFlashcards.Remove(card);
            }
        }

        private bool CanSaveBundle() => NewFlashcards.Count > 0 && !string.IsNullOrWhiteSpace(Title) && SelectedTopic != null;
        private async Task OnSaveBundle()
        {
            var newBundleDto = new CreateBundleDto
            {
                Title = this.Title,
                Description = this.Description ?? "",
                TopicId = this.SelectedTopic.TopicId,
                DifficultyLevel = this.Difficulty,
                NewFlashcards = this.NewFlashcards.ToList()
            };

            int userId = _currentUserService.CurrentUser.UserId;

            int newBundleId = await _bundleService.CreateNewBundleAsync(newBundleDto, userId);

            NavigationRequested?.Invoke($"FlashcardSession:{newBundleId}");
        }
    }
}