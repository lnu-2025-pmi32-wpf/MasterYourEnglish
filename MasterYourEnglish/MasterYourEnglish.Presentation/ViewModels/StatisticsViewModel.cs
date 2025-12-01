namespace MasterYourEnglish.Presentation.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using MasterYourEnglish.BLL.Interfaces;
    using MasterYourEnglish.BLL.Models.DTOs;
    using Microsoft.Extensions.Logging;

    public class StatisticsViewModel : ViewModelBase, IPageViewModel
    {
        private readonly IStatisticsService statisticsService;
        private readonly ICurrentUserService currentUserService;
        private readonly ILogger<StatisticsViewModel> logger;
        private bool isLoading;
        private StatisticsSummaryDto summary;
        private string searchTerm;
        private string selectedSortOption;
        private List<AttemptHistoryDto> allAttempts;

        public StatisticsViewModel(
            IStatisticsService statisticsService,
            ICurrentUserService currentUserService,
            ILogger<StatisticsViewModel> logger)
        {
            this.statisticsService = statisticsService;
            this.currentUserService = currentUserService;
            this.logger = logger;

            this.AttemptHistory = new ObservableCollection<AttemptHistoryDto>();
            this.allAttempts = new List<AttemptHistoryDto>();

            this.SortOptions = new List<string>
            {
                "Date (Newest)",
                "Date (Oldest)",
                "Score (High-Low)",
                "Score (Low-High)",
            };
            this.selectedSortOption = "Date (Newest)";
            this.logger.LogInformation("StatisticsViewModel initialized.");
        }

        public ObservableCollection<AttemptHistoryDto> AttemptHistory { get; }

        public List<string> SortOptions { get; }

        public StatisticsSummaryDto Summary
        {
            get => this.summary;
            set => this.SetProperty(ref this.summary, value);
        }

        public string SearchTerm
        {
            get => this.searchTerm;
            set
            {
                if (this.SetProperty(ref this.searchTerm, value))
                {
                    this.FilterAndSortHistory();
                }
            }
        }

        public string SelectedSortOption
        {
            get => this.selectedSortOption;
            set
            {
                if (this.SetProperty(ref this.selectedSortOption, value))
                {
                    this.FilterAndSortHistory();
                }
            }
        }

        public bool IsLoading
        {
            get => this.isLoading;
            set => this.SetProperty(ref this.isLoading, value);
        }

        public async Task LoadDataAsync()
        {
            if (this.IsLoading)
            {
                this.logger.LogDebug("LoadDataAsync skipped, already loading.");
                return;
            }

            this.IsLoading = true;
            this.logger.LogInformation("Starting to load statistics for user ID: {Id}", this.currentUserService.CurrentUser?.UserId ?? 0); // <-- Log Info

            try
            {
                var userId = this.currentUserService.CurrentUser?.UserId ?? 0;

                if (userId == 0)
                {
                    this.logger.LogWarning("LoadDataAsync skipped, current user ID is 0.");
                    return;
                }

                var history = await this.statisticsService.GetUserAttemptHistoryAsync(userId);
                this.allAttempts = history.ToList();

                this.Summary = await this.statisticsService.GetUserStatisticsSummaryAsync(userId);
                this.logger.LogInformation("Loaded statistics summary.");

                this.FilterAndSortHistory();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error loading statistics for user ID: {Id}", this.currentUserService.CurrentUser?.UserId ?? 0); // <-- Log Error

                MessageBox.Show($"Error loading statistics: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.IsLoading = false;
            }
        }

        private void FilterAndSortHistory()
        {
            this.logger.LogDebug(
                "Filtering and sorting history. SearchTerm: '{Search}', SortBy: {Sort}",
                this.searchTerm,
                this.selectedSortOption);

            var filtered = this.allAttempts.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(this.searchTerm))
            {
                var lower = this.searchTerm.ToLower();
                filtered = filtered.Where(a =>
                    (a.Title?.ToLower().Contains(lower) ?? false) ||
                    (a.TopicName?.ToLower().Contains(lower) ?? false));
            }

            filtered = this.selectedSortOption switch
            {
                "Date (Oldest)" => filtered.OrderBy(a => a.StartedAt),
                "Score (High-Low)" => filtered.OrderByDescending(a => a.Score ?? 0),
                "Score (Low-High)" => filtered.OrderBy(a => a.Score ?? 0),
                _ => filtered.OrderByDescending(a => a.StartedAt),
            };

            Application.Current.Dispatcher.Invoke(() =>
            {
                this.AttemptHistory.Clear();
                foreach (var attempt in filtered)
                {
                    this.AttemptHistory.Add(attempt);
                }
            });
            this.logger.LogInformation("History updated. Displaying {Count} records.", this.AttemptHistory.Count);
        }
    }
}