using System.Collections.ObjectModel;

namespace MasterYourEnglish.Presentation.ViewModels
{
    // This DTO should be in your BLL project, but for now,
    // we define it here so the ViewModel can use it.
    public class FlashcardBundleCardDto
    {
        public string CategoryName { get; set; } = "";
        public string BundleName { get; set; } = "";
    }

    public class FlashcardsViewModel : ViewModelBase
    {
        public ObservableCollection<FlashcardBundleCardDto> FlashcardBundles { get; }

        public FlashcardsViewModel(/*IFlashcardBundleService bundleService*/)
        {
            FlashcardBundles = new ObservableCollection<FlashcardBundleCardDto>();
            LoadFakeData(); // We'll replace this with the BLL call
        }

        private void LoadFakeData()
        {
            // Just using your wireframe's text
            FlashcardBundles.Add(new FlashcardBundleCardDto { CategoryName = "category name", BundleName = "Bundle name" });
            FlashcardBundles.Add(new FlashcardBundleCardDto { CategoryName = "category name", BundleName = "Bundle name" });
            FlashcardBundles.Add(new FlashcardBundleCardDto { CategoryName = "category name", BundleName = "Bundle name" });
            FlashcardBundles.Add(new FlashcardBundleCardDto { CategoryName = "category name", BundleName = "Bundle name" });
            FlashcardBundles.Add(new FlashcardBundleCardDto { CategoryName = "category name", BundleName = "Bundle name" });
            FlashcardBundles.Add(new FlashcardBundleCardDto { CategoryName = "category name", BundleName = "Bundle name" });
            FlashcardBundles.Add(new FlashcardBundleCardDto { CategoryName = "category name", BundleName = "Bundle name" });
            FlashcardBundles.Add(new FlashcardBundleCardDto { CategoryName = "category name", BundleName = "Bundle name" });
        }
    }
}