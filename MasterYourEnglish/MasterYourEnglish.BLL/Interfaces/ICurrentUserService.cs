using MasterYourEnglish.DAL.Entities;

namespace MasterYourEnglish.BLL.Interfaces
{
    public interface ICurrentUserService
    {
        User CurrentUser { get; }
        void SetCurrentUser(User user);
    }
}