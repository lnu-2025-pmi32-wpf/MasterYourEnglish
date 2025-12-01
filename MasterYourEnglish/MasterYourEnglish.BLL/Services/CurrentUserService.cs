namespace MasterYourEnglish.BLL.Services
{
    using MasterYourEnglish.BLL.Interfaces;
    using MasterYourEnglish.DAL.Entities;
    using Microsoft.Extensions.Logging;

    public class CurrentUserService : ICurrentUserService
    {
        private readonly ILogger<CurrentUserService> logger;

        public CurrentUserService(ILogger<CurrentUserService> logger)
        {
            this.logger = logger;
        }

        public User CurrentUser { get; private set; }

        public void SetCurrentUser(User user)
        {
            if (user != null)
            {
                this.logger.LogInformation("Setting current user: {UserName} (ID: {UserId})", user.UserName, user.UserId);
            }
            else
            {
                this.logger.LogWarning("Attempted to set current user to null.");
            }

            this.CurrentUser = user;
        }
    }
}