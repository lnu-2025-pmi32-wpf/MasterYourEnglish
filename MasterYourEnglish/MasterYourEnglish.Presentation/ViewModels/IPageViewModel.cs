using System.Threading.Tasks;

namespace MasterYourEnglish.Presentation.ViewModels
{
    public interface IPageViewModel
    {
        Task LoadDataAsync();
    }
}