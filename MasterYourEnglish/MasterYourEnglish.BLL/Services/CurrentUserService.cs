namespace MasterYourEnglish.BLL.Services
{
    using MasterYourEnglish.BLL.Interfaces;
    using MasterYourEnglish.DAL.Entities;

    public class CurrentUserService : ICurrentUserService
    {
        public User CurrentUser { get; private set; }

        public void SetCurrentUser(User user)
        {
            this.CurrentUser = user;
        }
    }
}