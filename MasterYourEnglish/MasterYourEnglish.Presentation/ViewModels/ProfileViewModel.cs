namespace MasterYourEnglish.Presentation.ViewModels
{
    using System.Threading.Tasks;
    using System.Windows.Input;
    using MasterYourEnglish.BLL.Interfaces;
    using MasterYourEnglish.Presentation.ViewModels.Commands;

    public class ProfileViewModel : ViewModelBase
    {
        private readonly IUserService userService;
        private readonly ICurrentUserService currentUserService;
        private string fullName;
        private string firstName;
        private string lastName;
        private string email;
        private string username;

        public ProfileViewModel(IUserService userService, ICurrentUserService currentUserService)
        {
            this.userService = userService;
            this.currentUserService = currentUserService;
            this.LoadUserProfile();
            this.SaveChangesCommand = new RelayCommand(async () => await this.OnSaveChanges());
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
                return;
            }

            this.FirstName = currentUser.FirstName;
            this.LastName = currentUser.LastName;
            this.Email = currentUser.Email;
            this.Username = currentUser.UserName;
        }

        private void UpdateFullName()
        {
            this.FullName = $"{this.FirstName} {this.LastName}";
        }

        private async Task OnSaveChanges()
        {
            int currentUserId = this.currentUserService.CurrentUser.UserId;
            bool success = await this.userService.UpdateProfileAsync(currentUserId, this.FirstName, this.LastName);
            if (success)
            {
                this.currentUserService.CurrentUser.FirstName = this.FirstName;
                this.currentUserService.CurrentUser.LastName = this.LastName;
            }
            else
            {
                // Show an error
            }
        }
    }
}