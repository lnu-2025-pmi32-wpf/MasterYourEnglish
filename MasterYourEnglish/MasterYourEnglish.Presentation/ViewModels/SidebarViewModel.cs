using MasterYourEnglish.Presentation.ViewModels.Commands;
using System.Windows.Input;

namespace MasterYourEnglish.Presentation.ViewModels
{
    public class SidebarViewModel : ViewModelBase
    {
        public event Action<ViewModelBase> NavigationRequested;
        public ProfileViewModel ProfileVm { get; }
        public FlashcardsViewModel FlashcardsVm { get; }
        public TestListViewModel TestListVm { get; }
        public StatisticsViewModel StatsVm { get; }
        public SettingsViewModel SettingsVm { get; }

        public ICommand NavigateToProfileCommand { get; }
        public ICommand NavigateToFlashcardsCommand { get; }
        public ICommand NavigateToTestsCommand { get; }
        public ICommand NavigateToStatisticsCommand { get; }
        public ICommand NavigateToSettingsCommand { get; }

        public SidebarViewModel(ProfileViewModel profileVm, FlashcardsViewModel flashcardsVm, TestListViewModel testListVm,
                                StatisticsViewModel statsVm, SettingsViewModel settingsVm)
        {
            ProfileVm = profileVm;
            FlashcardsVm = flashcardsVm;
            TestListVm = testListVm;
            StatsVm = statsVm;
            SettingsVm = settingsVm;

            NavigateToProfileCommand = new RelayCommand(
                () => NavigationRequested?.Invoke(ProfileVm)
            );

            NavigateToFlashcardsCommand = new RelayCommand(
                () => NavigationRequested?.Invoke(FlashcardsVm)
            );

            NavigateToTestsCommand = new RelayCommand(
                () => NavigationRequested?.Invoke(TestListVm)
            );

            NavigateToStatisticsCommand = new RelayCommand(
                () => NavigationRequested?.Invoke(StatsVm)
            );

            NavigateToSettingsCommand = new RelayCommand(
                () => NavigationRequested?.Invoke(SettingsVm)
            );
        }
    }
}