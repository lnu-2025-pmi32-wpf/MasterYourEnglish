namespace MasterYourEnglish.BLL.Services
{
    using System;
    using System.Threading.Tasks;
    using MasterYourEnglish.BLL.Interfaces;
    using MasterYourEnglish.DAL.Entities;
    using MasterYourEnglish.DAL.Interfaces;
    using Microsoft.Extensions.Logging;

    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly ILogger<UserService> logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            this.userRepository = userRepository;
            this.logger = logger;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            try
            {
                return await this.userRepository.GetByIdAsync(userId);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error getting user by ID {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> UpdateProfileAsync(int userId, string firstName, string lastName)
        {
            this.logger.LogInformation("Updating profile for user {UserId}", userId);
            try
            {
                var user = await this.userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    this.logger.LogWarning("User {UserId} not found for update.", userId);
                    return false;
                }

                user.FirstName = firstName;
                user.LastName = lastName;

                this.userRepository.UpdateAsync(user);
                this.logger.LogInformation("Profile updated successfully for user {UserId}", userId);

                return true;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error updating profile for user {UserId}", userId);
                throw;
            }
        }
    }
}