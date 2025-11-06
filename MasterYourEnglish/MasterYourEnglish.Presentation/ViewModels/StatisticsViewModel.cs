using System;
using System.Collections.ObjectModel;

namespace MasterYourEnglish.Presentation.ViewModels
{
    // This DTO should be in your BLL.
    // I'm defining it here so the file compiles.
    public class AttemptHistoryDto
    {
        public string CategoryName { get; set; } = "";
        public string BundleName { get; set; } = "";
        public DateTime Timestamp { get; set; }
    }

    public class StatisticsViewModel : ViewModelBase
    {
        // This is what the list will bind to
        public ObservableCollection<AttemptHistoryDto> AttemptHistory { get; }

        public StatisticsViewModel(/* We'll inject BLL services here */)
        {
            AttemptHistory = new ObservableCollection<AttemptHistoryDto>();
            LoadFakeData();
        }

        private void LoadFakeData()
        {
            // Fake data to fill the grid
            AttemptHistory.Add(new AttemptHistoryDto
            {
                CategoryName = "category name",
                BundleName = "Bundle name",
                Timestamp = DateTime.Now.AddDays(-1)
            });
            AttemptHistory.Add(new AttemptHistoryDto
            {
                CategoryName = "category name",
                BundleName = "Bundle name",
                Timestamp = DateTime.Now.AddDays(-2)
            });
            AttemptHistory.Add(new AttemptHistoryDto
            {
                CategoryName = "category name",
                BundleName = "Bundle name",
                Timestamp = DateTime.Now.AddDays(-3)
            });
            AttemptHistory.Add(new AttemptHistoryDto
            {
                CategoryName = "category name",
                BundleName = "Bundle name",
                Timestamp = DateTime.Now.AddDays(-4)
            });
        }
    }
}