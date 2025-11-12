using MasterYourEnglish.BLL.DTOs;
using MasterYourEnglish.BLL.Models.DTOs;

namespace MasterYourEnglish.Presentation.ViewModels
{
    public class TopicSelectionViewModel : ViewModelBase
    {
        public int TopicId { get; }
        public string Name { get; }

        private int _cardCount;
        public int CardCount
        {
            get => _cardCount;
            set => SetProperty(ref _cardCount, value);
        }

        public TopicSelectionViewModel(TopicDto topic)
        {
            TopicId = topic.TopicId;
            Name = topic.Name;
            CardCount = 0;
        }
    }
}