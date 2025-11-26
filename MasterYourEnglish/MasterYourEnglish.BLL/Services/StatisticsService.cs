namespace MasterYourEnglish.BLL.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MasterYourEnglish.BLL.Interfaces;
    using MasterYourEnglish.BLL.Models.DTOs;
    using MasterYourEnglish.DAL.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class StatisticsService : IStatisticsService
    {
        private readonly ITestAttemptRepository testAttemptRepository;
        private readonly IFlashcardsBundleAttemptRepository flashcardAttemptRepository;

        public StatisticsService(
            ITestAttemptRepository testAttemptRepository,
            IFlashcardsBundleAttemptRepository flashcardAttemptRepository)
        {
            this.testAttemptRepository = testAttemptRepository;
            this.flashcardAttemptRepository = flashcardAttemptRepository;
        }

        public async Task<IEnumerable<AttemptHistoryDto>> GetUserAttemptHistoryAsync(int userId)
        {
            var testAttempts = await this.testAttemptRepository.GetAttemptsForUserAsync(userId);

            var flashcardAttempts = await this.flashcardAttemptRepository.GetAttemptsForUserAsync(userId);

            var testHistory = testAttempts.Select(a => new AttemptHistoryDto
            {
                AttemptType = AttemptType.Test,
                AttemptId = a.AttemptId,
                Title = a.Test?.Title ?? "Unknown Test",
                TopicName = a.Test?.Topic?.Name,
                Score = a.Score,
                StartedAt = a.StartedAt,
                FinishedAt = a.FinishedAt,
                DifficultyLevel = a.Test?.DifficultyLevel
            });

            var flashcardHistory = flashcardAttempts.Select(a =>
            {
                float? score = null;
                
                if (a.FlashcardsAttemptAnswers != null && a.FlashcardsAttemptAnswers.Any())
                {
                    int totalCards = a.FlashcardsAttemptAnswers.Count;
                    int knownCards = a.FlashcardsAttemptAnswers.Count(ans => ans.IsKnown);
                    score = (float)knownCards / totalCards * 100;
                }

                return new AttemptHistoryDto
                {
                    AttemptType = AttemptType.Flashcard,
                    AttemptId = a.AttemptId,
                    Title = a.FlashcardsBundle?.Title ?? "Unknown Bundle",
                    TopicName = a.FlashcardsBundle?.Topic?.Name,
                    Score = score,
                    StartedAt = a.StartedAt,
                    FinishedAt = a.FinishedAt,
                    DifficultyLevel = a.FlashcardsBundle?.DifficultyLevel
                };
            });

            var allHistory = testHistory.Concat(flashcardHistory)
                .OrderByDescending(h => h.StartedAt)
                .ToList();

            return allHistory;
        }

        public async Task<StatisticsSummaryDto> GetUserStatisticsSummaryAsync(int userId)
        {
            var twoWeeksAgo = DateTime.UtcNow.AddDays(-14);

            var testAttempts = await this.testAttemptRepository.GetAttemptsForUserAsync(userId);
            var testAttemptsList = testAttempts.ToList();

            var flashcardAttempts = await this.flashcardAttemptRepository.GetAttemptsForUserAsync(userId);
            var flashcardAttemptsList = flashcardAttempts.ToList();

            int testsLast2Weeks = testAttemptsList.Count(a => a.StartedAt >= twoWeeksAgo);
            int flashcardsLast2Weeks = flashcardAttemptsList.Count(a => a.StartedAt >= twoWeeksAgo);

            float? averageScore = null;
            var completedTests = testAttemptsList.Where(a => a.FinishedAt.HasValue).ToList();
            if (completedTests.Any())
            {
                averageScore = completedTests.Average(a => a.Score);
            }

            return new StatisticsSummaryDto
            {
                TestsTakenLast2Weeks = testsLast2Weeks,
                FlashcardSessionsLast2Weeks = flashcardsLast2Weeks,
                AverageScore = averageScore,
                TotalAttempts = testAttemptsList.Count + flashcardAttemptsList.Count
            };
        }
    }
}
