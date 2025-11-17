namespace MasterYourEnglish.BLL.Interfaces
{
    using MasterYourEnglish.DAL.Entities;

    public interface ICurrentUserService
    {
        User CurrentUser { get; }

        void SetCurrentUser(User user);
    }
}