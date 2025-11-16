namespace MasterYourEnglish.Presentation.ViewModels
{
    using System;
    using System.Collections.ObjectModel;

    // This DTO should be in your BLL.
    // I'm defining it here so the file compiles.
    public class AttemptHistoryDto
    {
        public string CategoryName { get; set; } = string.Empty;

        public string BundleName { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; }
    }

    public class StatisticsViewModel : ViewModelBase
    {
        public StatisticsViewModel(/* We'll inject BLL services here */)
        {
            this.AttemptHistory = new ObservableCollection<AttemptHistoryDto>();
            this.LoadFakeData();
        }

        // This is what the list will bind to
        public ObservableCollection<AttemptHistoryDto> AttemptHistory { get; }

        private void LoadFakeData()
        {
            // Fake data to fill the grid
            this.AttemptHistory.Add(new AttemptHistoryDto
            {
                CategoryName = "category name",
                BundleName = "Bundle name",
                Timestamp = DateTime.Now.AddDays(-1),
            });
            this.AttemptHistory.Add(new AttemptHistoryDto
            {
                CategoryName = "category name",
                BundleName = "Bundle name",
                Timestamp = DateTime.Now.AddDays(-2),
            });
            this.AttemptHistory.Add(new AttemptHistoryDto
            {
                CategoryName = "category name",
                BundleName = "Bundle name",
                Timestamp = DateTime.Now.AddDays(-3),
            });
            this.AttemptHistory.Add(new AttemptHistoryDto
            {
                CategoryName = "category name",
                BundleName = "Bundle name",
                Timestamp = DateTime.Now.AddDays(-4),
            });
        }
    }
}