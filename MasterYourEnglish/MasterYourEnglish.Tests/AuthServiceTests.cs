using MasterYourEnglish.BLL.Interfaces;
using MasterYourEnglish.BLL.Models.DTOs;
using MasterYourEnglish.BLL.Services;
using MasterYourEnglish.DAL.Entities;
using MasterYourEnglish.DAL.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace MasterYourEnglish.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> userRepositoryMock;
        private readonly Mock<ICurrentUserService> currentUserServiceMock;
        private readonly Mock<ILogger<AuthService>> loggerMock;
        private readonly AuthService authService;

        public AuthServiceTests()
        {
            userRepositoryMock = new Mock<IUserRepository>();
            currentUserServiceMock = new Mock<ICurrentUserService>();
            loggerMock = new Mock<ILogger<AuthService>>();
            authService = new AuthService(userRepositoryMock.Object, currentUserServiceMock.Object, loggerMock.Object);
        }

        [Fact]
        public async Task LoginAsync_ValidUser_ReturnsTrue()
        {
            var user = new User { UserName = "user1", PasswordHash = BCrypt.Net.BCrypt.HashPassword("123") };
            userRepositoryMock.Setup(r => r.GetByUsernameAsync("user1")).ReturnsAsync(user);

            var result = await authService.LoginAsync("user1", "123");

            Assert.True(result);
        }

        [Fact]
        public async Task LoginAsync_WrongPassword_ReturnsFalse()
        {
            var user = new User { UserName = "user1", PasswordHash = BCrypt.Net.BCrypt.HashPassword("123") };
            userRepositoryMock.Setup(r => r.GetByUsernameAsync("user1")).ReturnsAsync(user);

            var result = await authService.LoginAsync("user1", "wrong");

            Assert.False(result);
        }

        [Fact]
        public async Task LoginAsync_UserNotFound_ReturnsFalse()
        {
            userRepositoryMock.Setup(r => r.GetByUsernameAsync("unknown")).ReturnsAsync((User)null);

            var result = await authService.LoginAsync("unknown", "any");

            Assert.False(result);
        }

        [Fact]
        public async Task RegisterAsync_NewUser_ReturnsTrue()
        {
            var dto = new RegisterDto { Username = "newuser", Password = "123" };
            userRepositoryMock.Setup(r => r.GetByUsernameAsync("newuser")).ReturnsAsync((User)null);

            var result = await authService.RegisterAsync(dto);

            Assert.True(result);
        }

        [Fact]
        public async Task RegisterAsync_UserExists_ReturnsFalse()
        {
            var dto = new RegisterDto { Username = "existuser", Password = "123" };
            userRepositoryMock.Setup(r => r.GetByUsernameAsync("existuser")).ReturnsAsync(new User());

            var result = await authService.RegisterAsync(dto);

            Assert.False(result);
        }
    }
}
