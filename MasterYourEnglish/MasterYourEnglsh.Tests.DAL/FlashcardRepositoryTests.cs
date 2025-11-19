using MasterYourEnglish.DAL.Repositories;
using MasterYourEnglish.DAL.Entities;
using Xunit;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;

public class FlashcardRepositoryTests : DalTestBase
{
    [Fact]
    public async Task AddAsync_ShouldInsertNewFlashcard()
    {
        var newCard = new Flashcard
        {
            Word = "TestWord",
            Meaning = "TestDefinition",
            CreatedBy = 1,
            TopicId = 1,
            CreatedAt = DateTime.Now
        };
        var repository = new FlashcardRepository(Context);

        await repository.AddAsync(newCard);
        await Context.SaveChangesAsync();

        var cardInDb = await Context.Flashcards.FirstOrDefaultAsync(f => f.Word == "TestWord");
        Assert.NotNull(cardInDb);
        Assert.Equal("TestDefinition", cardInDb.Meaning);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyFlashcard()
    {
        var originalCard = new Flashcard
        {
            Word = "OldWord",
            Meaning = "OldDefinition",
            CreatedBy = 1,
            TopicId = 1,
            CreatedAt = DateTime.Now
        };
        Context.Flashcards.Add(originalCard);
        await Context.SaveChangesAsync();

        var repository = new FlashcardRepository(Context);

        originalCard.Meaning = "UpdatedDefinition";

        await repository.UpdateAsync(originalCard);
        await Context.SaveChangesAsync();

        var updatedCard = await Context.Flashcards.FindAsync(originalCard.FlashcardId);
        Assert.NotNull(updatedCard);
        Assert.Equal("UpdatedDefinition", updatedCard.Meaning);
    }

    [Fact]
    public async Task RemoveAsync_ShouldDeleteFlashcard()
    {
        var cardToDelete = new Flashcard
        {
            Word = "ToErase",
            Meaning = "Will be deleted",
            CreatedBy = 1,
            TopicId = 1,
            CreatedAt = DateTime.Now
        };
        Context.Flashcards.Add(cardToDelete);
        await Context.SaveChangesAsync();

        var repository = new FlashcardRepository(Context);

        Context.Flashcards.Remove(cardToDelete);
        await Context.SaveChangesAsync();

        var cardInDb = await Context.Flashcards.FindAsync(cardToDelete.FlashcardId);
        Assert.Null(cardInDb);
    }

    [Fact]
    public async Task GetFlashcardsByTopic_ShouldReturnOnlyFlashcardsForGivenTopicId()
    {
        const int TARGET_TOPIC_ID = 10;
        const int OTHER_TOPIC_ID = 20;

        Context.Flashcards.AddRange(
            new Flashcard { Word = "Apple", Meaning = "фрукт", TopicId = TARGET_TOPIC_ID, CreatedAt = DateTime.Now },
            new Flashcard { Word = "Banana", Meaning = "фрукт", TopicId = OTHER_TOPIC_ID, CreatedAt = DateTime.Now },
            new Flashcard { Word = "Cherry", Meaning = "ягода", TopicId = TARGET_TOPIC_ID, CreatedAt = DateTime.Now.AddDays(-1) }
        );
        await Context.SaveChangesAsync();

        var repository = new FlashcardRepository(Context);

        var result = await repository.GetFlashcardsByTopic(TARGET_TOPIC_ID);

        Assert.Equal(2, result.Count());
        Assert.True(result.All(f => f.TopicId == TARGET_TOPIC_ID));
        Assert.Equal("Apple", result.First().Word);
    }

    [Fact]
    public async Task GetFlashcardsByDifficultyLevel_ShouldFilterByLevel()
    {
        const string TARGET_LEVEL = "A2";

        Context.Flashcards.AddRange(
            new Flashcard { Word = "Easy", DifficultyLevel = TARGET_LEVEL, CreatedAt = DateTime.Now },
            new Flashcard { Word = "Hard", DifficultyLevel = "C1", CreatedAt = DateTime.Now }
        );
        await Context.SaveChangesAsync();

        var repository = new FlashcardRepository(Context);

        var result = await repository.GetFlashcardsByDifficultyLevel(TARGET_LEVEL);

        Assert.Single(result);
        Assert.True(result.First().DifficultyLevel == TARGET_LEVEL);
    }

    [Fact]
    public async Task GetSavedFlashcardsForUserAsync_ShouldReturnSavedCardsWithTopicIncluded()
    {
        const int TARGET_USER_ID = 99;
        var topic = new Topic { TopicId = 5, Name = "TestTopic" };
        var card1 = new Flashcard { FlashcardId = 100, Word = "SavedCard", TopicId = 5, Topic = topic, CreatedAt = DateTime.Now };
        var card2 = new Flashcard { FlashcardId = 101, Word = "UnsavedCard", TopicId = 5, Topic = topic, CreatedAt = DateTime.Now };

        var user = new User { UserId = TARGET_USER_ID, UserName = "testUser", Email = "a@b.com", FirstName = "F", LastName = "L", PasswordHash = "hash" };

        Context.Topics.Add(topic);
        Context.Users.Add(user);
        Context.Flashcards.AddRange(card1, card2);

        Context.SavedFlashcards.Add(new SavedFlashcard { UserId = TARGET_USER_ID, FlashcardId = 100 });
        await Context.SaveChangesAsync();

        var repository = new FlashcardRepository(Context);

        var result = await repository.GetSavedFlashcardsForUserAsync(TARGET_USER_ID);

        Assert.Single(result);
        var savedCard = result.First();
        Assert.Equal("SavedCard", savedCard.Word);

        Assert.NotNull(savedCard.Topic);
        Assert.Equal("TestTopic", savedCard.Topic.Name);
    }

    [Fact]
    public async Task GetFlashcardsByCriteriaAsync_ShouldFilterByTopicAndLevelsAndLimitCount()
    {
        const int TARGET_TOPIC_ID = 30;
        var levels = new List<string> { "B1", "B2" };

        Context.Flashcards.AddRange(
            new Flashcard { Word = "B1_T30_1", DifficultyLevel = "B1", TopicId = TARGET_TOPIC_ID, CreatedAt = DateTime.Now },
            new Flashcard { Word = "B2_T30_2", DifficultyLevel = "B2", TopicId = TARGET_TOPIC_ID, CreatedAt = DateTime.Now },
            new Flashcard { Word = "C1_T30_3", DifficultyLevel = "C1", TopicId = TARGET_TOPIC_ID, CreatedAt = DateTime.Now },
            new Flashcard { Word = "B1_T40_4", DifficultyLevel = "B1", TopicId = 40, CreatedAt = DateTime.Now }
        );
        await Context.SaveChangesAsync();

        var repository = new FlashcardRepository(Context);

        var result = await repository.GetFlashcardsByCriteriaAsync(TARGET_TOPIC_ID, levels, count: 1);

        Assert.Single(result);
        Assert.True(levels.Contains(result.First().DifficultyLevel));
        Assert.Equal(TARGET_TOPIC_ID, result.First().TopicId);
    }
}