using MasterYourEnglish.DAL.Repositories;
using MasterYourEnglish.DAL.Entities;
using Xunit;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

public class TestAttemptRepositoryTests : DalTestBase
{
    // ----------------------------------------------------------------------
    //                   ТЕСТ 1: ФІЛЬТРАЦІЯ ЗА КОРИСТУВАЧЕМ
    // ----------------------------------------------------------------------

    [Fact]
    public async Task GetAttemptsForUserAsync_ShouldFilterByUserIdAndOrderByStartedAtDescending()
    {
        const int TARGET_USER_ID = 10;
        const int OTHER_USER_ID = 20;

        Context.TestAttempts.AddRange(
            // Спроба 1: Цільовий користувач, новіша
            new TestAttempt { AttemptId = 1, UserId = TARGET_USER_ID, StartedAt = DateTime.Now },
            // Спроба 2: Інший користувач (фільтрується)
            new TestAttempt { AttemptId = 2, UserId = OTHER_USER_ID, StartedAt = DateTime.Now.AddHours(-1) },
            // Спроба 3: Цільовий користувач, старіша
            new TestAttempt { AttemptId = 3, UserId = TARGET_USER_ID, StartedAt = DateTime.Now.AddHours(-2) }
        );
        await Context.SaveChangesAsync();

        var repository = new TestAttemptRepository(Context);

        // ACT
        var result = await repository.GetAttemptsForUserAsync(TARGET_USER_ID);

        // ASSERT: Фільтрація та сортування
        Assert.Equal(2, result.Count());
        Assert.True(result.All(a => a.UserId == TARGET_USER_ID));

        // Перевірка сортування: від новішого (1) до старішого (3)
        Assert.Equal(1, result.First().AttemptId);
        Assert.Equal(3, result.Last().AttemptId);
    }

    // ----------------------------------------------------------------------
    //                   ТЕСТ 2: СКЛАДНЕ ЗАВАНТАЖЕННЯ (INCLUDE)
    // ----------------------------------------------------------------------

    [Fact]
    public async Task GetAttemptWithAnswersAsync_ShouldLoadAllRelatedDetails()
    {
        // ARRANGE
        const int TARGET_ATTEMPT_ID = 500;

        // Створення сутностей
        var testEntity = new Test { TestId = 10, Title = "Main Test" };
        var question = new Question { QuestionId = 20, Text = "Q1", QuestionType = "Multi" };
        var correctOption = new QuestionOption { OptionId = 30, Text = "Correct Opt", IsCorrect = true };

        var attemptAnswer = new TestAttemptAnswer
        {
            AttemptAnswersId = 40,
            QuestionId = 20,
            Question = question,
            IsCorrect = true,
            // Додаємо обраний варіант (для перевірки складеного ThenInclude)
            SelectedOptions = new List<TestAttemptAnswerSelectedOption>
            {
                new TestAttemptAnswerSelectedOption { SelectedOptionId = 30, SelectedOption = correctOption }
            }
        };

        var attempt = new TestAttempt
        {
            AttemptId = TARGET_ATTEMPT_ID,
            TestId = 10,
            Test = testEntity,
            TestAttemptsAnswers = new List<TestAttemptAnswer> { attemptAnswer }
        };

        // Вставка даних в контекст
        Context.Tests.Add(testEntity);
        Context.Questions.Add(question);
        Context.QuestionOptions.Add(correctOption);
        Context.TestAttemptAnswers.Add(attemptAnswer);
        Context.TestAttempts.Add(attempt);
        // Необхідно додати зв'язок SelectedOption вручну
        Context.TestAttemptAnswerSelectedOptions.Add(attemptAnswer.SelectedOptions.First());

        await Context.SaveChangesAsync();

        var repository = new TestAttemptRepository(Context);

        // ACT
        var result = await repository.GetAttemptWithAnswersAsync(TARGET_ATTEMPT_ID);

        // ASSERT: Перевірка Eager Loading
        Assert.NotNull(result);

        // 1. Include(a => a.Test)
        Assert.NotNull(result.Test);
        Assert.Equal("Main Test", result.Test.Title);

        // 2. Include(a => a.TestAttemptsAnswers)
        Assert.NotEmpty(result.TestAttemptsAnswers);
        var answer = result.TestAttemptsAnswers.First();

        // 3. ThenInclude(ans => ans.Question)
        Assert.NotNull(answer.Question);
        Assert.Equal("Q1", answer.Question.Text);

        // 4. ThenInclude(ans => ans.SelectedOptions)
        Assert.NotEmpty(answer.SelectedOptions);

        // 5. Подальший ThenInclude(so => so.SelectedOption)
        var selectedOpt = answer.SelectedOptions.First();
        Assert.NotNull(selectedOpt.SelectedOption);
        Assert.Equal("Correct Opt", selectedOpt.SelectedOption.Text);
    }
}