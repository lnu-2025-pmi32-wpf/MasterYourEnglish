namespace MasterYourEnglish.Presentation.ViewModels
{
    using System;
    using System.Windows.Input;
    using MasterYourEnglish.Presentation.ViewModels.Commands;

    public class SessionResultsViewModel : ViewModelBase
    {
        private int knownCount;
        private int totalCount;

        public SessionResultsViewModel()
        {
            this.FinishCommand = new RelayCommand(
                p => this.NavigationRequested?.Invoke("Flashcards"));
        }

        public event Action<string> NavigationRequested;

        public int KnownCount
        {
            get => this.knownCount;
            set => this.SetProperty(ref this.knownCount, value);
        }

        public int TotalCount
        {
            get => this.totalCount;
            set => this.SetProperty(ref this.totalCount, value);
        }

        public ICommand FinishCommand { get; }

        public void ShowResults(int known, int total)
        {
            this.KnownCount = known;
            this.TotalCount = total;
        }
    }
}