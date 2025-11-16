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

    public class GenerateBundleViewModel : ViewModelBase, IPageViewModel
    {
        private readonly ITopicService topicService;
        private readonly IFlashcardBundleService bundleService;
        private readonly ICurrentUserService currentUserService;
        private string selectedMinLevel;
        private string selectedMaxLevel;

        public GenerateBundleViewModel(ITopicService topicService, IFlashcardBundleService bundleService, ICurrentUserService currentUserService)
        {
            this.topicService = topicService;
            this.bundleService = bundleService;
            this.currentUserService = currentUserService;
            this.Topics = new ObservableCollection<TopicSelectionViewModel>();
            this.Levels = new List<string> { "A1", "A2", "B1", "B2", "C1" };
            this.selectedMinLevel = this.Levels.First();
            this.selectedMaxLevel = this.Levels.Last();
            this.GenerateCommand = new RelayCommand(async p => await this.OnGenerate());
            this.CancelCommand = new RelayCommand(p => this.NavigationRequested?.Invoke("Flashcards"));
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
            var topicDtos = await this.topicService.GetAllTopicsAsync();
            this.Topics.Clear();
            foreach (var dto in topicDtos)
            {
                this.Topics.Add(new TopicSelectionViewModel(dto));
            }
        }

        private async Task OnGenerate()
        {
            int minIndex = this.Levels.IndexOf(this.selectedMinLevel);
            int maxIndex = this.Levels.IndexOf(this.selectedMaxLevel);
            var selectedLevels = this.Levels.Skip(minIndex).Take(maxIndex - minIndex + 1).ToList();
            var topicRequests = this.Topics
                .Where(t => t.CardCount > 0)
                .ToDictionary(t => t.TopicId, t => t.CardCount);

            if (topicRequests.Count == 0)
            {
                // Show an error
                return;
            }

            int userId = this.currentUserService.CurrentUser.UserId;
            List<FlashcardSessionDto> generatedCards = await this.bundleService.GetGeneratedSessionAsync(userId, selectedLevels, topicRequests);

            if (generatedCards.Any())
            {
                this.SessionGenerated?.Invoke(generatedCards);
            }
            else
            {
                // Show "No cards found" error
            }
        }
    }
}