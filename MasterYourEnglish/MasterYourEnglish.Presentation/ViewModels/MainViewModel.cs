using System;

namespace MasterYourEnglish.Presentation.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public SidebarViewModel SidebarVm { get; }

        public MainViewModel(SidebarViewModel sidebarViewModel)
        {
            SidebarVm = sidebarViewModel;
            SidebarVm.NavigationRequested += OnNavigationRequested;

            CurrentViewModel = SidebarVm.ProfileVm;
        }

        private void OnNavigationRequested(ViewModelBase viewModel)
        {
            CurrentViewModel = viewModel;
        }
    }
}