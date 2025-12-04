using MasterYourEnglish.BLL.Services;
using MasterYourEnglish.DAL.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MasterYourEnglish.Tests
{
    public class CurrentUserServiceTests
    {
        private readonly Mock<ILogger<CurrentUserService>> loggerMock = new();

        [Fact]
        public void SetCurrentUser_ShouldSetCurrentUserProperty()
        {

            var service = new CurrentUserService(loggerMock.Object);
            var user = new User { UserName = "testuser" };

            service.SetCurrentUser(user);

            Assert.Equal(user, service.CurrentUser);
        }

        [Fact]
        public void CurrentUser_InitiallyNull()
        {

            var service = new CurrentUserService(loggerMock.Object);

            Assert.Null(service.CurrentUser);
        }
    }
}
