using MasterYourEnglish.BLL.Services;
using MasterYourEnglish.DAL.Entities;
using Xunit;

namespace MasterYourEnglish.Tests
{
    public class CurrentUserServiceTests
    {
        [Fact]
        public void SetCurrentUser_ShouldSetCurrentUserProperty()
        {

            var service = new CurrentUserService();
            var user = new User { UserName = "testuser" };

            service.SetCurrentUser(user);

            Assert.Equal(user, service.CurrentUser);
        }

        [Fact]
        public void CurrentUser_InitiallyNull()
        {

            var service = new CurrentUserService();

            Assert.Null(service.CurrentUser);
        }
    }
}
