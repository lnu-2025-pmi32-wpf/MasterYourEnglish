using MasterYourEnglish.BLL.Interfaces;
using MasterYourEnglish.Presentation.ViewModels.Commands;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MasterYourEnglish.Presentation.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    {
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUserService;

        private string _fullName;
        public string FullName
        {
            get => _fullName;
            set => SetProperty(ref _fullName, value);
        }

        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set
            {
                SetProperty(ref _firstName, value);
                UpdateFullName();
            }
        }

        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set
            {
                SetProperty(ref _lastName, value);
                UpdateFullName();
            }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _username;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public ICommand SaveChangesCommand { get; }

        public ProfileViewModel(IUserService userService, ICurrentUserService currentUserService)
        {
            _userService = userService;
            _currentUserService = currentUserService;

            LoadUserProfile();

            SaveChangesCommand = new RelayCommand(async () => await OnSaveChanges());
        }

        private void LoadUserProfile()
        {
            var currentUser = _currentUserService.CurrentUser;
            if (currentUser == null)
            {
                FirstName = "Error";
                LastName = "User not found";
                return;
            }

            FirstName = currentUser.FirstName;
            LastName = currentUser.LastName;
            Email = currentUser.Email;
            Username = currentUser.UserName;

        }

        private void UpdateFullName()
        {
            FullName = $"{FirstName} {LastName}";
        }

        private async Task OnSaveChanges()
        {
            int currentUserId = _currentUserService.CurrentUser.UserId;

            bool success = await _userService.UpdateProfileAsync(currentUserId, this.FirstName, this.LastName);

            if (success)
            {
                _currentUserService.CurrentUser.FirstName = this.FirstName;
                _currentUserService.CurrentUser.LastName = this.LastName;
            }
            else
            {
                // Show an error
            }
        }
    }
}