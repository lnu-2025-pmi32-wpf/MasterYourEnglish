using MasterYourEnglish.Presentation.ViewModels.Commands;
using System;
using System.Windows.Input;

namespace MasterYourEnglish.Presentation.ViewModels
{
    public class SessionResultsViewModel : ViewModelBase
    {
        public event Action<string> NavigationRequested;

        private int _knownCount;
        public int KnownCount
        {
            get => _knownCount;
            set => SetProperty(ref _knownCount, value);
        }

        private int _totalCount;
        public int TotalCount
        {
            get => _totalCount;
            set => SetProperty(ref _totalCount, value);
        }

        public ICommand FinishCommand { get; }

        public SessionResultsViewModel()
        {
            FinishCommand = new RelayCommand(
                p => NavigationRequested?.Invoke("Flashcards")
            );
        }
        public void ShowResults(int known, int total)
        {
            KnownCount = known;
            TotalCount = total;
        }
    }
}