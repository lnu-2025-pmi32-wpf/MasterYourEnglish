using System.Collections.ObjectModel;

namespace MasterYourEnglish.Presentation.ViewModels
{
    public class TestCardDto
    {
        public string CategoryName { get; set; } = "";
        public string TestName { get; set; } = "";
    }

    public class TestListViewModel : ViewModelBase
    {
        public ObservableCollection<TestCardDto> TestCards { get; }

        public TestListViewModel(/* ITestService testService */)
        {
            // _testService = testService;
            TestCards = new ObservableCollection<TestCardDto>();
            LoadFakeData();
        }

        private async void LoadFakeData()
        {
            // var tests = await _testService.GetPublishedTestsForDashboardAsync();
            // TestCards.Clear();
            // foreach (var test in tests)
            // {
            //     TestCards.Add(test);
            // }

            // --- TEMPORARY FAKE DATA ---
            TestCards.Add(new TestCardDto { CategoryName = "Computer Science", TestName = "Grammar Quiz" });
            TestCards.Add(new TestCardDto { CategoryName = "Animals", TestName = "Past Tense Test" });
            TestCards.Add(new TestCardDto { CategoryName = "Family Affairs", TestName = "Advanced Idioms" });
            TestCards.Add(new TestCardDto { CategoryName = "Computer Science", TestName = "Conditionals" });
        }
    }
}