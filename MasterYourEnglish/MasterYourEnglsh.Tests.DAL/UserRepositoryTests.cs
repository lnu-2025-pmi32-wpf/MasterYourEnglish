using MasterYourEnglish.DAL.Repositories;
using MasterYourEnglish.DAL.Entities;
using Xunit;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;

public class UserRepositoryTests : DalTestBase
{
    [Fact]
    public async Task AddAsync_ShouldInsertNewUser()
    {
        var newUser = new User
        {
            FirstName = "Test",
            LastName = "User",
            UserName = "testuser1",
            Email = "test1@test.com",
            PasswordHash = "hash123"
        };
        var repository = new UserRepository(Context);

        await repository.AddAsync(newUser);
        await Context.SaveChangesAsync();

        var userInDb = await Context.Users.FirstOrDefaultAsync(u => u.UserName == "testuser1");
        Assert.NotNull(userInDb);
        Assert.Equal("Test", userInDb.FirstName);
    }

    [Fact]
    public async Task Remove_ShouldDeleteUser()
    {
        var userToDelete = new User
        {
            FirstName = "Del",
            LastName = "User",
            UserName = "deluser",
            Email = "del@test.com",
            PasswordHash = "hash123"
        };
        Context.Users.Add(userToDelete);
        await Context.SaveChangesAsync();

        var repository = new UserRepository(Context);

        Context.Users.Remove(userToDelete);
        await Context.SaveChangesAsync();

        var userInDb = await Context.Users.FindAsync(userToDelete.UserId);
        Assert.Null(userInDb);
    }

    [Fact]
    public async Task GetByUsernameAsync_ShouldReturnCorrectUser()
    {
        const string TARGET_USERNAME = "findme";

        var userToFind = new User
        {
            FirstName = "Find",
            LastName = "Me",
            UserName = TARGET_USERNAME,
            Email = "find@me.com",
            PasswordHash = "hash"
        };
        var otherUser = new User
        {
            FirstName = "Other",
            LastName = "User",
            UserName = "other",
            Email = "other@test.com",
            PasswordHash = "hash"
        };

        Context.Users.AddRange(userToFind, otherUser);
        await Context.SaveChangesAsync();

        var repository = new UserRepository(Context);

        var result = await repository.GetByUsernameAsync(TARGET_USERNAME);

        Assert.NotNull(result);
        Assert.Equal(TARGET_USERNAME, result.UserName);
        Assert.Equal("Find", result.FirstName);
    }
}