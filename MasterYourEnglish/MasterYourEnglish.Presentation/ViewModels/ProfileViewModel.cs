namespace MasterYourEnglish.Presentation.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using MasterYourEnglish.BLL.Interfaces;
    using MasterYourEnglish.Presentation.ViewModels.Commands;
    using Microsoft.Extensions.Logging;

    public class ProfileViewModel : ViewModelBase
    {
        private readonly IUserService userService;
        private readonly ICurrentUserService currentUserService;
        private readonly ILogger<ProfileViewModel> logger;
        private string fullName;
        private string firstName;
        private string lastName;
        private string email;
        private string username;

        public ProfileViewModel(
            IUserService userService,
            ICurrentUserService currentUserService,
            ILogger<ProfileViewModel> logger)
        {
            this.userService = userService;
            this.currentUserService = currentUserService;
            this.logger = logger;

            this.LoadUserProfile();
            this.SaveChangesCommand = new RelayCommand(async (p) => await this.OnSaveChanges());
            this.logger.LogInformation("ProfileViewModel initialized.");
        }

        public string FullName
        {
            get => this.fullName;
            set => this.SetProperty(ref this.fullName, value);
        }

        public string FirstName
        {
            get => this.firstName;
            set
            {
                this.SetProperty(ref this.firstName, value);
                this.UpdateFullName();
            }
        }

        public string LastName
        {
            get => this.lastName;
            set
            {
                this.SetProperty(ref this.lastName, value);
                this.UpdateFullName();
            }
        }

        public string Email
        {
            get => this.email;
            set => this.SetProperty(ref this.email, value);
        }

        public string Username
        {
            get => this.username;
            set => this.SetProperty(ref this.username, value);
        }

        public ICommand SaveChangesCommand { get; }

        private void LoadUserProfile()
        {
            var currentUser = this.currentUserService.CurrentUser;
            if (currentUser == null)
            {
                this.FirstName = "Error";
                this.LastName = "User not found";
                this.logger.LogError("CurrentUserService returned null user in LoadUserProfile.");
                return;
            }

            this.FirstName = currentUser.FirstName;
            this.LastName = currentUser.LastName;
            this.Email = currentUser.Email;
            this.Username = currentUser.UserName;
            this.logger.LogInformation("User profile loaded for user: {Username}", currentUser.UserName);
        }

        private void UpdateFullName()
        {
            this.FullName = $"{this.FirstName} {this.LastName}";
        }

        private async Task OnSaveChanges()
        {
            this.logger.LogInformation("Attempting to save profile changes for user: {Username}", this.Username);

            try
            {
                if (this.currentUserService.CurrentUser == null)
                {
                    this.logger.LogWarning("Cannot save changes: Current user object is null.");
                    return;
                }

                int currentUserId = this.currentUserService.CurrentUser.UserId;
                bool success = await this.userService.UpdateProfileAsync(currentUserId, this.FirstName, this.LastName);

                if (success)
                {
                    this.currentUserService.CurrentUser.FirstName = this.FirstName;
                    this.currentUserService.CurrentUser.LastName = this.LastName;
                    this.logger.LogInformation("Profile updated successfully for user ID: {Id}", currentUserId);
                }
                else
                {
                    this.logger.LogWarning("Profile update failed (service returned false) for user ID: {Id}", currentUserId);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Exception during profile save for user: {Username}", this.Username);
            }
        }
    }
}