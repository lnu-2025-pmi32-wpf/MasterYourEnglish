namespace MasterYourEnglish.BLL.Services
{
    using System.Threading.Tasks;
    using MasterYourEnglish.BLL.Interfaces;
    using MasterYourEnglish.DAL.Entities;
    using MasterYourEnglish.DAL.Interfaces;

    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await this.userRepository.GetByIdAsync(userId);
        }

        public async Task<bool> UpdateProfileAsync(int userId, string firstName, string lastName)
        {
            var user = await this.userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.FirstName = firstName;
            user.LastName = lastName;

            this.userRepository.UpdateAsync(user);

            return true;
        }
    }
}