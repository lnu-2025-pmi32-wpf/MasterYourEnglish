using MasterYourEnglish.BLL.DTOs;
using MasterYourEnglish.BLL.Interfaces;
using MasterYourEnglish.BLL.Models.DTOs;
using MasterYourEnglish.DAL.Entities;
using MasterYourEnglish.DAL.Interfaces;

namespace MasterYourEnglish.BLL.Services
{
    public class TestService : ITestService
    {
        private readonly ITestRepository _testRepository;
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<QuestionOption> _optionRepository;
        private readonly IRepository<TestQuestion> _testQuestionRepository;
        private readonly ITestAttemptRepository _attemptRepository;
        private readonly IRepository<TestAttemptAnswer> _attemptAnswerRepository;
        private readonly IRepository<TestAttemptAnswerSelectedOption> _selectedOptionRepository;

        public TestService(
            ITestRepository testRepository,
            IRepository<Question> questionRepository,
            IRepository<QuestionOption> optionRepository,
            IRepository<TestQuestion> testQuestionRepository,
            ITestAttemptRepository attemptRepository,
            IRepository<TestAttemptAnswer> attemptAnswerRepository,
            IRepository<TestAttemptAnswerSelectedOption> selectedOptionRepository)
        {
            _testRepository = testRepository;
            _questionRepository = questionRepository;
            _optionRepository = optionRepository;
            _testQuestionRepository = testQuestionRepository;
            _attemptRepository = attemptRepository;
            _attemptAnswerRepository = attemptAnswerRepository;
            _selectedOptionRepository = selectedOptionRepository;
        }

        public async Task<IEnumerable<TestCardDto>> GetPublishedTestsAsync(string searchTerm, string sortBy, bool ascending)
        {
            var tests = await _testRepository.GetPublishedTestsWithDetailsAsync();

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
                Description = t.Description ?? ""
            });
        }

        public async Task<TestSessionDto> GetTestSessionAsync(int testId)
        {
            var test = await _testRepository.GetTestWithDetailsAsync(testId);
            if (test == null) return null;

            return new TestSessionDto
            {
                TestId = test.TestId,
                Title = test.Title,
                Description = test.Description ?? "",
                Questions = test.TestQuestions.OrderBy(tq => tq.Position).Select(tq => new TestQuestionDto
                {
                    QuestionId = tq.Question.QuestionId,
                    Text = tq.Question.Text,
                    Options = tq.Question.QuestionOptions.Select(o => new TestOptionDto
                    {
                        OptionId = o.OptionId,
                        Text = o.Text
                    }).ToList()
                }).ToList()
            };
        }

        public async Task<int> SubmitTestAttemptAsync(int testId, int userId, Dictionary<int, int> answers)
        {
            int correctCount = 0;
            foreach (var answer in answers)
            {
                var option = await _optionRepository.GetByIdAsync(answer.Value);
                if (option != null && option.IsCorrect) correctCount++;
            }

            var attempt = new TestAttempt
            {
                TestId = testId,
                UserId = userId,
                StartedAt = DateTime.UtcNow.AddMinutes(-10),
                FinishedAt = DateTime.UtcNow,
                Score = correctCount
            };
            await _attemptRepository.AddAsync(attempt);

            foreach (var answer in answers)
            {
                var attemptAnswer = new TestAttemptAnswer
                {
                    AttemptId = attempt.AttemptId,
                    QuestionId = answer.Key,
                    SelectedOptionId = answer.Value
                };
                await _attemptAnswerRepository.AddAsync(attemptAnswer);
            }

            return correctCount;
        }

        public async Task<int> CreateNewTestAsync(CreateTestDto testDto, int userId)
        {
            var newTest = new Test
            {
                Title = testDto.Title,
                Description = testDto.Description,
                TopicId = testDto.TopicId,
                DifficultyLevel = testDto.DifficultyLevel,
                CreatedAt = DateTime.UtcNow,
                IsPublished = false,
                IsUserCreated = true,
                CreatedBy = userId,
                TotalQuestionsCount = testDto.NewQuestions.Count
            };
            await _testRepository.AddAsync(newTest);

            int position = 1;
            foreach (var qDto in testDto.NewQuestions)
            {
                var newQ = new Question
                {
                    Text = qDto.Text,
                    QuestionType = "SingleChoice",
                    DifficultyLevel = testDto.DifficultyLevel,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId,
                    TopicId = testDto.TopicId
                };
                await _questionRepository.AddAsync(newQ);

                await _testQuestionRepository.AddAsync(new TestQuestion { TestId = newTest.TestId, QuestionId = newQ.QuestionId, Position = position++ });

                for (int i = 0; i < qDto.Options.Count; i++)
                {
                    await _optionRepository.AddAsync(new QuestionOption
                    {
                        QuestionId = newQ.QuestionId,
                        Text = qDto.Options[i].Text,
                        IsCorrect = (i == qDto.CorrectOptionIndex)
                    });
                }
            }
            return newTest.TestId;
        }

        public async Task<List<TestSessionDto>> GetGeneratedTestSessionAsync(int userId, List<string> levels, Dictionary<int, int> topicRequests)
        {
            var allQuestions = new List<Question>();

            foreach (var req in topicRequests)
            {
                var questions = await _questionRepository.FindAsync(x =>
                    x.TopicId == req.Key &&
                    levels.Contains(x.DifficultyLevel));

                var taken = questions.OrderBy(x => Guid.NewGuid()).Take(req.Value).ToList();
                allQuestions.AddRange(taken);
            }

            if (!allQuestions.Any()) return new List<TestSessionDto>();

            var qIds = allQuestions.Select(q => q.QuestionId).ToList();
            var allOptions = await _optionRepository.FindAsync(o => qIds.Contains(o.QuestionId));

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
                                        .ToList()
                }).ToList()
            };

            return new List<TestSessionDto> { sessionDto };
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