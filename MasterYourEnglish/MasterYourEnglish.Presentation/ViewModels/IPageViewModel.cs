namespace MasterYourEnglish.Presentation.ViewModels
{
    using System.Threading.Tasks;

    public interface IPageViewModel
    {
        Task LoadDataAsync();
    }
}