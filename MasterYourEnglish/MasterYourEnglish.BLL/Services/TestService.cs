namespace MasterYourEnglish.BLL.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MasterYourEnglish.BLL.DTOs;
    using MasterYourEnglish.BLL.Interfaces;
    using MasterYourEnglish.BLL.Models.DTOs;
    using MasterYourEnglish.DAL.Entities;
    using MasterYourEnglish.DAL.Interfaces;
    using Microsoft.Extensions.Logging;

    public class TestService : ITestService
    {
        private readonly ITestRepository testRepository;
        private readonly IRepository<Question> questionRepository;
        private readonly IRepository<QuestionOption> optionRepository;
        private readonly IRepository<TestQuestion> testQuestionRepository;
        private readonly ITestAttemptRepository attemptRepository;
        private readonly IRepository<TestAttemptAnswer> attemptAnswerRepository;
        private readonly IRepository<TestAttemptAnswerSelectedOption> selectedOptionRepository;
        private readonly ILogger<TestService> logger;

        public TestService(
            ITestRepository testRepository,
            IRepository<Question> questionRepository,
            IRepository<QuestionOption> optionRepository,
            IRepository<TestQuestion> testQuestionRepository,
            ITestAttemptRepository attemptRepository,
            IRepository<TestAttemptAnswer> attemptAnswerRepository,
            IRepository<TestAttemptAnswerSelectedOption> selectedOptionRepository,
            ILogger<TestService> logger)
        {
            this.testRepository = testRepository;
            this.questionRepository = questionRepository;
            this.optionRepository = optionRepository;
            this.testQuestionRepository = testQuestionRepository;
            this.attemptRepository = attemptRepository;
            this.attemptAnswerRepository = attemptAnswerRepository;
            this.selectedOptionRepository = selectedOptionRepository;
            this.logger = logger;
        }

        public async Task<IEnumerable<TestCardDto>> GetPublishedTestsAsync(string searchTerm, string sortBy, bool ascending)
        {
            this.logger.LogInformation("Getting published tests. Search: '{SearchTerm}', Sort: {SortBy}", searchTerm, sortBy);
            try
            {
                var tests = await this.testRepository.GetPublishedTestsWithDetailsAsync();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    string lower = searchTerm.ToLower();
                    tests = tests.Where(t => t.Title.ToLower().Contains(lower));
                }

                switch (sortBy)
                {
                    case "Level":
                        tests = ascending ? tests.OrderBy(t => t.DifficultyLevel) : tests.OrderByDescending(t => t.DifficultyLevel);
                        break;
                    default:
                        tests = ascending ? tests.OrderBy(t => t.Title) : tests.OrderByDescending(t => t.Title);
                        break;
                }

                return tests.Select(t => new TestCardDto
                {
                    TestId = t.TestId,
                    TestName = t.Title,
                    CategoryName = t.Topic?.Name ?? "General",
                    DifficultyLevel = t.DifficultyLevel ?? "N/A",
                    Description = t.Description ?? string.Empty,
                });
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error getting published tests.");
                throw;
            }
        }

        public async Task<TestSessionDto> GetTestSessionAsync(int testId)
        {
            this.logger.LogInformation("Getting test session for test ID {TestId}", testId);
            try
            {
                var test = await this.testRepository.GetTestWithDetailsAsync(testId);
                if (test == null)
                {
                    this.logger.LogWarning("Test with ID {TestId} not found.", testId);
                    return null;
                }

                return new TestSessionDto
                {
                    TestId = test.TestId,
                    Title = test.Title,
                    Description = test.Description ?? string.Empty,
                    Questions = test.TestQuestions.OrderBy(tq => tq.Position).Select(tq => new TestQuestionDto
                    {
                        QuestionId = tq.Question.QuestionId,
                        Text = tq.Question.Text,
                        QuestionType = tq.Question.QuestionType ?? "SingleChoice",
                        Options = tq.Question.QuestionOptions.Select(o => new TestOptionDto
                        {
                            OptionId = o.OptionId,
                            Text = o.Text,
                            MatchingText = o.MatchingText ?? string.Empty,
                        }).ToList(),
                    }).ToList(),
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error getting test session for ID {TestId}", testId);
                throw;
            }
        }

        public async Task<int> SubmitTestAttemptAsync(int testId, int userId, Dictionary<int, int> answers)
        {
            this.logger.LogInformation("Submitting test attempt. TestID: {TestId}, UserID: {UserId}", testId, userId);
            try
            {
                int correctCount = 0;
                int totalQuestions = answers.Count;

                foreach (var answer in answers)
                {
                    if (answer.Value == -1)
                    {
                        continue;
                    }

                    var option = await this.optionRepository.GetByIdAsync(answer.Value);
                    if (option == null)
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(option.MatchingText))
                    {
                        correctCount++;
                    }
                    else if (option.IsCorrect)
                    {
                        correctCount++;
                    }
                }

                float scorePercentage = totalQuestions > 0 ? (float)correctCount / totalQuestions * 100 : 0;

                var attempt = new TestAttempt
                {
                    TestId = testId,
                    UserId = userId,
                    StartedAt = DateTime.UtcNow.AddMinutes(-10),
                    FinishedAt = DateTime.UtcNow,
                    Score = scorePercentage,
                };
                await this.attemptRepository.AddAsync(attempt);

                foreach (var answer in answers)
                {
                    // Skip invalid answers (-1 used for incorrect matching questions)
                    if (answer.Value <= 0)
                    {
                        continue;
                    }

                    var attemptAnswer = new TestAttemptAnswer
                    {
                        AttemptId = attempt.AttemptId,
                        QuestionId = answer.Key,
                        SelectedOptionId = answer.Value,
                    };
                    await this.attemptAnswerRepository.AddAsync(attemptAnswer);
                }

                this.logger.LogInformation("Test attempt submitted successfully. Score: {Score}", scorePercentage);
                return correctCount;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error submitting test attempt for TestID {TestId}, UserID {UserId}", testId, userId);
                throw;
            }
        }

        public async Task<int> CreateNewTestAsync(CreateTestDto testDto, int userId)
        {
            this.logger.LogInformation("Creating new test '{Title}' by UserID {UserId}", testDto.Title, userId);
            try
            {
                var newTest = new Test
                {
                    Title = testDto.Title,
                    Description = testDto.Description,
                    TopicId = testDto.TopicId,
                    DifficultyLevel = testDto.DifficultyLevel,
                    CreatedAt = DateTime.UtcNow,
                    IsPublished = true,
                    IsUserCreated = true,
                    CreatedBy = userId,
                    TotalQuestionsCount = testDto.NewQuestions.Count,
                };
                await this.testRepository.AddAsync(newTest);

                int position = 1;
                foreach (var qDto in testDto.NewQuestions)
                {
                    var newQ = new Question
                    {
                        Text = qDto.Text,
                        QuestionType = qDto.QuestionType ?? "SingleChoice",
                        DifficultyLevel = testDto.DifficultyLevel,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = userId,
                        TopicId = testDto.TopicId,
                    };
                    await this.questionRepository.AddAsync(newQ);

                    await this.testQuestionRepository.AddAsync(new TestQuestion { TestId = newTest.TestId, QuestionId = newQ.QuestionId, Position = position++ });

                    for (int i = 0; i < qDto.Options.Count; i++)
                    {
                        bool isCorrect = qDto.Options[i].IsCorrect
                            || (qDto.CorrectOptionIndices != null && qDto.CorrectOptionIndices.Contains(i))
                            || i == qDto.CorrectOptionIndex;

                        await this.optionRepository.AddAsync(new QuestionOption
                        {
                            QuestionId = newQ.QuestionId,
                            Text = qDto.Options[i].Text,
                            IsCorrect = isCorrect,
                            MatchingText = qDto.Options[i].MatchingText ?? string.Empty,
                        });
                    }
                }

                this.logger.LogInformation("Test created successfully. ID: {TestId}", newTest.TestId);
                return newTest.TestId;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error creating new test by UserID {UserId}", userId);
                throw;
            }
        }

        public async Task<List<TestSessionDto>> GetGeneratedTestSessionAsync(int userId, List<string> levels, Dictionary<int, int> topicRequests)
        {
            this.logger.LogInformation("Generating test session for UserID {UserId}", userId);
            try
            {
                var allQuestions = new List<Question>();

                foreach (var req in topicRequests)
                {
                    var questions = await this.questionRepository.FindAsync(x =>
                        x.TopicId == req.Key &&
                        levels.Contains(x.DifficultyLevel) &&
                        x.CreatedBy == null);

                    var taken = questions.OrderBy(x => Guid.NewGuid()).Take(req.Value).ToList();
                    allQuestions.AddRange(taken);
                }

                if (!allQuestions.Any())
                {
                    this.logger.LogWarning("No questions found for generated test session.");
                    return new List<TestSessionDto>();
                }

                var qIds = allQuestions.Select(q => q.QuestionId).ToList();
                var allOptions = await this.optionRepository.FindAsync(o => qIds.Contains(o.QuestionId));

                var sessionDto = new TestSessionDto
                {
                    TestId = 0,
                    Title = "Generated Test",
                    Description = "Custom generated test session.",
                    Questions = allQuestions.Select(q => new TestQuestionDto
                    {
                        QuestionId = q.QuestionId,
                        Text = q.Text,
                        Options = allOptions.Where(o => o.QuestionId == q.QuestionId)
                                            .Select(o => new TestOptionDto { OptionId = o.OptionId, Text = o.Text })
                                            .ToList(),
                    }).ToList(),
                };

                return new List<TestSessionDto> { sessionDto };
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error generating test session for UserID {UserId}", userId);
                throw;
            }
        }

        public Task<IEnumerable<TestCardDto>> GetPublishedTestsForDashboardAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TestCardDto>> GetPublishedTestsByTopicAsync(int topicId)
        {
            throw new NotImplementedException();
        }
    }
}