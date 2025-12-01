namespace MasterYourEnglish.Presentation.ViewModels
{
    using Microsoft.Extensions.Logging;

    public class SettingsViewModel : ViewModelBase
    {
        private readonly ILogger<SettingsViewModel> logger;

        // add theme-switching logic later
        public SettingsViewModel(ILogger<SettingsViewModel> logger)
        {
            this.logger = logger;
            this.logger.LogInformation("SettingsViewModel initialized.");
        }
    }
}