using MasterYourEnglish.BLL.Interfaces;
using MasterYourEnglish.DAL.Entities;

namespace MasterYourEnglish.BLL.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public User CurrentUser { get; private set; }

        public void SetCurrentUser(User user)
        {
            this.CurrentUser = user;
        }
    }
}