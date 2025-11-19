using MasterYourEnglish.DAL.Repositories;
using MasterYourEnglish.DAL.Entities;
using Xunit;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class FlashcardBundleRepositoryTests : DalTestBase
{
    [Fact]
    public async Task GetByDifficultyLevelAsync_ShouldFilterByLevelAndPublishedAndSortByTitle()
    {
        var b1 = new FlashcardBundle
        {
            FlashcardsBundleId = 1,
            DifficultyLevel = "A1",
            IsPublished = true,
            Title = "C-Bundle"
        };

        var b2 = new FlashcardBundle
        {
            FlashcardsBundleId = 2,
            DifficultyLevel = "A1",
            IsPublished = false,
            Title = "A-Bundle"
        };

        var b3 = new FlashcardBundle
        {
            FlashcardsBundleId = 3,
            DifficultyLevel = "B2",
            IsPublished = true,
            Title = "B-Bundle"
        };

        Context.FlashcardBundles.AddRange(b1, b2, b3);
        await Context.SaveChangesAsync();

        var repository = new FlashcardBundleRepository(Context);

        var result = await repository.GetFlashcardBundlesByDifficultyLevelAsync("A1");

        Assert.Single(result);

        var expectedBundle = result.First();
        Assert.Equal("C-Bundle", expectedBundle.Title);
        Assert.True(expectedBundle.IsPublished);
    }

    [Fact]
    public async Task AddAsync_ShouldInsertFlashcardBundle()
    {
        var newBundle = new FlashcardBundle
        {
            Title = "New Test Bundle",
            DifficultyLevel = "C1",
            IsPublished = true
        };
        var repository = new FlashcardBundleRepository(Context);

        await repository.AddAsync(newBundle);
        await Context.SaveChangesAsync();

        var bundleInDb = await Context.FlashcardBundles.FirstOrDefaultAsync(b => b.Title == "New Test Bundle");
        Assert.NotNull(bundleInDb);
        Assert.Equal("C1", bundleInDb.DifficultyLevel);
    }
}