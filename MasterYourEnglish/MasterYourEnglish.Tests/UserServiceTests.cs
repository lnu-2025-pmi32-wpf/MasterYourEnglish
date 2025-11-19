using System.Threading.Tasks;
using MasterYourEnglish.BLL.Services;
using MasterYourEnglish.DAL.Entities;
using MasterYourEnglish.DAL.Interfaces;
using Moq;
using Xunit;

namespace MasterYourEnglish.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> userRepoMock = new();
        private readonly UserService service;

        public UserServiceTests()
        {
            service = new UserService(userRepoMock.Object);
        }

        [Fact]
        public async Task GetUserByIdAsync_UserExists_ReturnsUser()
        {

            var user = new User { UserName = "testuser" };
            userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

            var result = await service.GetUserByIdAsync(1);

            Assert.Equal("testuser", result.UserName);
        }

        [Fact]
        public async Task UpdateProfileAsync_UserExists_UpdatesAndReturnsTrue()
        {
         
            var user = new User { UserName = "oldname", FirstName = "Old", LastName = "Name" };
            userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

            var result = await service.UpdateProfileAsync(1, "NewFirst", "NewLast");

            Assert.True(result);
            Assert.Equal("NewFirst", user.FirstName);
            Assert.Equal("NewLast", user.LastName);
            userRepoMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task UpdateProfileAsync_UserNotFound_ReturnsFalse()
        {

            userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((User)null);

            var result = await service.UpdateProfileAsync(1, "NewFirst", "NewLast");

            Assert.False(result);
            userRepoMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
        }
    }
}
