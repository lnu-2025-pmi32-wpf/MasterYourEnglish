using MasterYourEnglish.DAL.Repositories;
using MasterYourEnglish.DAL.Entities;
using Xunit;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

public class TestRepositoryTests : DalTestBase
{


    [Fact]
    public async Task AddAsync_ShouldInsertNewTest()
    {
        var newTest = new Test { Title = "New Test", CreatedAt = DateTime.Now, TotalQuestionsCount = 10, DifficultyLevel = "A1" };
        var repository = new TestRepository(Context);

        await repository.AddAsync(newTest);
        await Context.SaveChangesAsync();

        var testInDb = await Context.Tests.FirstOrDefaultAsync(t => t.Title == "New Test");
        Assert.NotNull(testInDb);
        Assert.Equal(10, testInDb.TotalQuestionsCount);
    }

    [Fact]
    public async Task Remove_ShouldDeleteTest()
    {
        var testToDelete = new Test { Title = "To Erase", CreatedAt = DateTime.Now, TotalQuestionsCount = 5 };
        Context.Tests.Add(testToDelete);
        await Context.SaveChangesAsync();

        var repository = new TestRepository(Context);

        Context.Tests.Remove(testToDelete);
        await Context.SaveChangesAsync();

        var testInDb = await Context.Tests.FindAsync(testToDelete.TestId);
        Assert.Null(testInDb);
    }


    [Fact]
    public async Task GetPublishedTestsWithDetailsAsync_ShouldFilterAndIncludeTopic()
    {

        var topic = new Topic { TopicId = 1, Name = "Physics" };
        var publishedTest = new Test { TestId = 100, Title = "Pub Test", IsPublished = true, TopicId = 1, Topic = topic, CreatedAt = DateTime.Now };
        var unpublishedTest = new Test { TestId = 101, Title = "Unpub Test", IsPublished = false, TopicId = 1, Topic = topic, CreatedAt = DateTime.Now };

        Context.Topics.Add(topic);
        Context.Tests.AddRange(publishedTest, unpublishedTest);
        await Context.SaveChangesAsync();

        var repository = new TestRepository(Context);


        var result = await repository.GetPublishedTestsWithDetailsAsync();


        Assert.Single(result);
        var actualTest = result.First();

        Assert.Equal("Pub Test", actualTest.Title);

        Assert.NotNull(actualTest.Topic);
        Assert.Equal("Physics", actualTest.Topic.Name);
    }
}