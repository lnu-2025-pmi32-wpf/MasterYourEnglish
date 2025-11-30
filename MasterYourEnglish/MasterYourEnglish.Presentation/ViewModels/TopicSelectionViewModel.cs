namespace MasterYourEnglish.Presentation.ViewModels
{
    using MasterYourEnglish.BLL.DTOs;
    using MasterYourEnglish.BLL.Models.DTOs;
    using Microsoft.Extensions.Logging;

    public class TopicSelectionViewModel : ViewModelBase
    {
        private readonly ILogger<TopicSelectionViewModel> logger;
        private int cardCount;

        public TopicSelectionViewModel(TopicDto topic, ILogger<TopicSelectionViewModel> logger)
        {
            this.logger = logger;

            this.TopicId = topic.TopicId;
            this.Name = topic.Name;
            this.CardCount = 0;

            this.logger.LogInformation("TopicSelectionViewModel initialized for topic: {Name}", this.Name);
        }

        public int TopicId { get; }

        public string Name { get; }

        public int CardCount
        {
            get => this.cardCount;
            set => this.SetProperty(ref this.cardCount, value);
        }
    }
}