using MasterYourEnglish.DAL.Repositories;
using MasterYourEnglish.DAL.Entities;
using Xunit;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

public class FlashcardsBundleAttemptRepositoryTests : DalTestBase
{
    [Fact]
    public async Task GetAttemptsForUserAsync_ShouldFilterByUserIdAndOrderByStartedAt()
    {
        const int TARGET_USER_ID = 1;
        const int OTHER_USER_ID = 2;

        Context.FlashcardBundleAttempts.AddRange(
            new FlashcardBundleAttempt { AttemptId = 1, UserId = TARGET_USER_ID, StartedAt = DateTime.Now },
            new FlashcardBundleAttempt { AttemptId = 2, UserId = OTHER_USER_ID, StartedAt = DateTime.Now.AddHours(-1) },
            new FlashcardBundleAttempt { AttemptId = 3, UserId = TARGET_USER_ID, StartedAt = DateTime.Now.AddHours(-2) }
        );
        await Context.SaveChangesAsync();

        var repository = new FlashcardsBundleAttemptRepository(Context);

        var result = await repository.GetAttemptsForUserAsync(TARGET_USER_ID);

        Assert.Equal(2, result.Count());
        Assert.True(result.All(a => a.UserId == TARGET_USER_ID));
        Assert.Equal(1, result.First().AttemptId);
        Assert.Equal(3, result.Last().AttemptId);
    }

    [Fact]
    public async Task GetAttemptWithDetailsAsync_ShouldLoadBundleAndAnswersWithFlashcard()
    {
        const int TARGET_ATTEMPT_ID = 50;

        var bundle = new FlashcardBundle { FlashcardsBundleId = 10, Title = "TestBundle" };
        var flashcard = new Flashcard { FlashcardId = 20, Word = "DetailWord" };

        var attempt = new FlashcardBundleAttempt
        {
            AttemptId = TARGET_ATTEMPT_ID,
            UserId = 1,
            FlashcardsBundleId = 10,
            FlashcardsBundle = bundle,
            FlashcardsAttemptAnswers = new List<FlashcardAttemptAnswer>
            {
                new FlashcardAttemptAnswer { AnswerId = 100, FlashcardId = 20, Flashcard = flashcard }
            }
        };

        Context.FlashcardBundles.Add(bundle);
        Context.Flashcards.Add(flashcard);
        Context.FlashcardAttemptAnswers.Add(attempt.FlashcardsAttemptAnswers.First());
        Context.FlashcardBundleAttempts.Add(attempt);
        await Context.SaveChangesAsync();

        var repository = new FlashcardsBundleAttemptRepository(Context);

        var result = await repository.GetAttemptWithDetailsAsync(TARGET_ATTEMPT_ID);

        Assert.NotNull(result);
        Assert.NotNull(result.FlashcardsBundle);
        Assert.NotNull(result.FlashcardsAttemptAnswers);

        var answer = result.FlashcardsAttemptAnswers.First();
        Assert.NotNull(answer.Flashcard);
        Assert.Equal("DetailWord", answer.Flashcard.Word);
    }
}