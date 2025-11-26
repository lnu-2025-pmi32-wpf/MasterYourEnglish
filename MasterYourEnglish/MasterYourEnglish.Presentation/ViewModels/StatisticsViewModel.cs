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

    public class StatisticsViewModel : ViewModelBase, IPageViewModel
    {
        private readonly IStatisticsService statisticsService;
        private readonly ICurrentUserService currentUserService;
        private bool isLoading;
        private StatisticsSummaryDto summary;
        private string searchTerm;
        private string selectedSortOption;
        private List<AttemptHistoryDto> allAttempts;

        public StatisticsViewModel(
            IStatisticsService statisticsService,
            ICurrentUserService currentUserService)
        {
            this.statisticsService = statisticsService;
            this.currentUserService = currentUserService;
            this.AttemptHistory = new ObservableCollection<AttemptHistoryDto>();
            this.allAttempts = new List<AttemptHistoryDto>();
            
            this.SortOptions = new List<string>
            {
                "Date (Newest)",
                "Date (Oldest)",
                "Score (High-Low)",
                "Score (Low-High)"
            };
            this.selectedSortOption = "Date (Newest)";
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
                return;
            }

            this.IsLoading = true;

            try
            {
                var userId = this.currentUserService.CurrentUser?.UserId ?? 0;
                
                if (userId == 0)
                {
                    return;
                }

                var history = await this.statisticsService.GetUserAttemptHistoryAsync(userId);
                this.allAttempts = history.ToList();

                this.Summary = await this.statisticsService.GetUserStatisticsSummaryAsync(userId);

                this.FilterAndSortHistory();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading statistics: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.IsLoading = false;
            }
        }

        private void FilterAndSortHistory()
        {
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
        }
    }
}