using MasterYourEnglish.DAL.Repositories;
using MasterYourEnglish.DAL.Entities;
using Xunit;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

public class QuestionRepositoryTests : DalTestBase
{
    [Fact]
    public async Task AddAsync_ShouldInsertNewQuestion()
    {
        var newQuestion = new Question
        {
            Text = "New Question Text",
            QuestionType = "SingleChoice",
            CreatedAt = DateTime.Now
        };
        var repository = new QuestionRepository(Context);

        await repository.AddAsync(newQuestion);
        await Context.SaveChangesAsync();

        var questionInDb = await Context.Questions.FirstOrDefaultAsync(q => q.Text == "New Question Text");
        Assert.NotNull(questionInDb);
        Assert.Equal("SingleChoice", questionInDb.QuestionType);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyQuestion()
    {
        var originalQuestion = new Question
        {
            Text = "Original Text",
            QuestionType = "MultipleChoice",
            CreatedAt = DateTime.Now
        };
        Context.Questions.Add(originalQuestion);
        await Context.SaveChangesAsync();

        var repository = new QuestionRepository(Context);

        originalQuestion.Text = "Updated Text";
        originalQuestion.QuestionType = "SingleChoice";

        await repository.UpdateAsync(originalQuestion);
        await Context.SaveChangesAsync();

        var updatedQuestion = await Context.Questions.FindAsync(originalQuestion.QuestionId);
        Assert.NotNull(updatedQuestion);
        Assert.Equal("Updated Text", updatedQuestion.Text);
        Assert.Equal("SingleChoice", updatedQuestion.QuestionType);
    }

    [Fact]
    public async Task Remove_ShouldDeleteQuestion()
    {
        var questionToDelete = new Question
        {
            Text = "Text to delete",
            QuestionType = "SingleChoice",
            CreatedAt = DateTime.Now
        };
        Context.Questions.Add(questionToDelete);
        await Context.SaveChangesAsync();

        var repository = new QuestionRepository(Context);

        Context.Questions.Remove(questionToDelete);
        await Context.SaveChangesAsync();

        var questionInDb = await Context.Questions.FindAsync(questionToDelete.QuestionId);
        Assert.Null(questionInDb);
    }

    [Fact]
    public async Task GetQuestionsByDifficultyLevel_ShouldFilterByLevelAndOrderByCreatedAt()
    {
        const string TARGET_LEVEL = "B1";
        const string OTHER_LEVEL = "C1";

        Context.Questions.AddRange(
            new Question { QuestionId = 1, Text = "Q1", DifficultyLevel = TARGET_LEVEL, QuestionType = "SingleChoice", CreatedAt = DateTime.Now.AddDays(-3) },
            new Question { QuestionId = 2, Text = "Q2", DifficultyLevel = OTHER_LEVEL, QuestionType = "SingleChoice", CreatedAt = DateTime.Now.AddDays(-1) },
            new Question { QuestionId = 3, Text = "Q3", DifficultyLevel = TARGET_LEVEL, QuestionType = "SingleChoice", CreatedAt = DateTime.Now.AddDays(-2) }
        );
        await Context.SaveChangesAsync();

        var repository = new QuestionRepository(Context);

        var result = await repository.GetQuestionsByDifficultyLevel(TARGET_LEVEL);

        Assert.Equal(2, result.Count());
        Assert.True(result.All(q => q.DifficultyLevel == TARGET_LEVEL));

        Assert.Equal(1, result.First().QuestionId);
        Assert.Equal(3, result.Last().QuestionId);
    }

    [Fact]
    public async Task GetQuestionsByTopic_ShouldFilterByTopicIdAndOrderByCreatedAt()
    {
        const int TARGET_TOPIC_ID = 5;
        const int OTHER_TOPIC_ID = 6;

        Context.Questions.AddRange(
            new Question { QuestionId = 10, Text = "T1", TopicId = TARGET_TOPIC_ID, QuestionType = "SingleChoice", CreatedAt = DateTime.Now.AddDays(-1) },
            new Question { QuestionId = 20, Text = "T2", TopicId = OTHER_TOPIC_ID, QuestionType = "SingleChoice", CreatedAt = DateTime.Now },
            new Question { QuestionId = 30, Text = "T3", TopicId = TARGET_TOPIC_ID, QuestionType = "SingleChoice", CreatedAt = DateTime.Now.AddDays(-2) }
        );
        await Context.SaveChangesAsync();

        var repository = new QuestionRepository(Context);

        var result = await repository.GetQuestionsByTopic(TARGET_TOPIC_ID);

        Assert.Equal(2, result.Count());
        Assert.True(result.All(q => q.TopicId == TARGET_TOPIC_ID));

        Assert.Equal(30, result.First().QuestionId);
        Assert.Equal(10, result.Last().QuestionId);
    }
}