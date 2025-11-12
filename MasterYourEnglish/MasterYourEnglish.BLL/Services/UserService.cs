using MasterYourEnglish.BLL.Interfaces;
using MasterYourEnglish.DAL.Entities;
using MasterYourEnglish.DAL.Interfaces;
using System.Threading.Tasks;

namespace MasterYourEnglish.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<bool> UpdateProfileAsync(int userId, string firstName, string lastName)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.FirstName = firstName;
            user.LastName = lastName;

            _userRepository.UpdateAsync(user); 

            return true;
        }
    }
}