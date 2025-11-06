using MasterYourEnglish.Presentation.ViewModels.Commands;
using System.Windows.Input;

namespace MasterYourEnglish.Presentation.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    {
        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
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

        private string _role;
        public string Role
        {
            get => _role;
            set => SetProperty(ref _role, value);
        }

        public ICommand SaveChangesCommand { get; }

        public ProfileViewModel(/* We'll inject a BLL UserService here */)
        {
            // Load real data from BLL, for now we use fake data
            LoadFakeData();

            SaveChangesCommand = new RelayCommand(OnSaveChanges);
        }

        private void LoadFakeData()
        {
            FirstName = "John";
            LastName = "Doe";
            Email = "john.doe@email.com";
            Username = "John Doe";
            Role = "Admin";
        }

        private void OnSaveChanges()
        {
            // This is where you'll call your BLL UserService
            // await _userService.UpdateProfileAsync( ... );
        }
    }
}