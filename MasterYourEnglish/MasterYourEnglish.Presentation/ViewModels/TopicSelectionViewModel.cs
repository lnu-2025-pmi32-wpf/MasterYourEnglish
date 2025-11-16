namespace MasterYourEnglish.Presentation.ViewModels
{
    using MasterYourEnglish.BLL.DTOs;
    using MasterYourEnglish.BLL.Models.DTOs;

    public class TopicSelectionViewModel : ViewModelBase
    {
        private int cardCount;

        public TopicSelectionViewModel(TopicDto topic)
        {
            this.TopicId = topic.TopicId;
            this.Name = topic.Name;
            this.CardCount = 0;
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