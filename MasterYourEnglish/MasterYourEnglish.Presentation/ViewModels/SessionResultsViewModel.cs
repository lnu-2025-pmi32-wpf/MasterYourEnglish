namespace MasterYourEnglish.Presentation.ViewModels
{
    using System;
    using System.Windows.Input;
    using MasterYourEnglish.Presentation.ViewModels.Commands;
    using Microsoft.Extensions.Logging;

    public class SessionResultsViewModel : ViewModelBase
    {
        private readonly ILogger<SessionResultsViewModel> logger;
        private int knownCount;
        private int totalCount;
        private string returnNavigationKey = "Flashcards";

        public SessionResultsViewModel(ILogger<SessionResultsViewModel> logger)
        {
            this.logger = logger;

            this.FinishCommand = new RelayCommand(
                p =>
                {
                    this.logger.LogInformation("Session results finished. Navigating to: {Key}", this.returnNavigationKey);
                    this.NavigationRequested?.Invoke(this.returnNavigationKey);
                });

            this.logger.LogInformation("SessionResultsViewModel initialized.");
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

        public void ShowResults(int known, int total, string returnKey)
        {
            this.KnownCount = known;
            this.TotalCount = total;
            this.returnNavigationKey = returnKey;

            this.logger.LogInformation(
                "Displaying results: Known={Known}, Total={Total}, ReturnKey={Key}",
                known,
                total,
                returnKey);
        }
    }
}