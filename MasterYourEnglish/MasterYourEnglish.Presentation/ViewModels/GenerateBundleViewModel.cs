using MasterYourEnglish.BLL.DTOs;
using MasterYourEnglish.BLL.Interfaces;
using MasterYourEnglish.Presentation.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MasterYourEnglish.Presentation.ViewModels
{
    public class GenerateBundleViewModel : ViewModelBase, IPageViewModel
    {
        private readonly ITopicService _topicService;
        private readonly IFlashcardBundleService _bundleService;
        private readonly ICurrentUserService _currentUserService;

        public event Action<List<FlashcardSessionDto>> SessionGenerated;

        public event Action<string> NavigationRequested;


        public ObservableCollection<TopicSelectionViewModel> Topics { get; }
        public List<string> Levels { get; }

        private string _selectedMinLevel;
        public string SelectedMinLevel
        {
            get => _selectedMinLevel;
            set => SetProperty(ref _selectedMinLevel, value);
        }

        private string _selectedMaxLevel;
        public string SelectedMaxLevel
        {
            get => _selectedMaxLevel;
            set => SetProperty(ref _selectedMaxLevel, value);
        }

        public ICommand GenerateCommand { get; }
        public ICommand CancelCommand { get; }

        public GenerateBundleViewModel(ITopicService topicService, IFlashcardBundleService bundleService, ICurrentUserService currentUserService)
        {
            _topicService = topicService;
            _bundleService = bundleService;
            _currentUserService = currentUserService;

            Topics = new ObservableCollection<TopicSelectionViewModel>();
            Levels = new List<string> { "A1", "A2", "B1", "B2", "C1" };
            _selectedMinLevel = Levels.First();
            _selectedMaxLevel = Levels.Last();

            GenerateCommand = new RelayCommand(async p => await OnGenerate());
            CancelCommand = new RelayCommand(p => NavigationRequested?.Invoke("Flashcards"));
        }

        public async Task LoadDataAsync()
        {
            var topicDtos = await _topicService.GetAllTopicsAsync();
            Topics.Clear();
            foreach (var dto in topicDtos)
            {
                Topics.Add(new TopicSelectionViewModel(dto));
            }
        }

        private async Task OnGenerate()
        {
            int minIndex = Levels.IndexOf(_selectedMinLevel);
            int maxIndex = Levels.IndexOf(_selectedMaxLevel);
            var selectedLevels = Levels.Skip(minIndex).Take(maxIndex - minIndex + 1).ToList();

            var topicRequests = Topics
                .Where(t => t.CardCount > 0)
                .ToDictionary(t => t.TopicId, t => t.CardCount);

            if (topicRequests.Count == 0)
            {
                // Show an error
                return;
            }

            int userId = _currentUserService.CurrentUser.UserId;
            List<FlashcardSessionDto> generatedCards = await _bundleService.GetGeneratedSessionAsync(userId, selectedLevels, topicRequests);

            if (generatedCards.Any())
            {
                SessionGenerated?.Invoke(generatedCards);
            }
            else
            {
                // Show "No cards found" error
            }
        }
    }
}