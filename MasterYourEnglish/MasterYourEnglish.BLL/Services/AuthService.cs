namespace MasterYourEnglish.BLL.Services
{
    using System;
    using System.Threading.Tasks;
    using MasterYourEnglish.BLL.Interfaces;
    using MasterYourEnglish.BLL.Models.DTOs;
    using MasterYourEnglish.DAL.Entities;
    using MasterYourEnglish.DAL.Interfaces;
    using Microsoft.Extensions.Logging;

    public class AuthService : IAuthService
    {
        private readonly IUserRepository userRepository;
        private readonly ICurrentUserService currentUserService;
        private readonly ILogger<AuthService> logger;

        public AuthService(IUserRepository userRepository, ICurrentUserService currentUserService, ILogger<AuthService> logger)
        {
            this.userRepository = userRepository;
            this.currentUserService = currentUserService;
            this.logger = logger;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            this.logger.LogInformation("Attempting login for user: {Username}", username);
            try
            {
                var user = await this.userRepository.GetByUsernameAsync(username);
                if (user == null)
                {
                    this.logger.LogWarning("Login failed. User {Username} not found.", username);
                    return false;
                }

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

                if (isPasswordValid)
                {
                    this.currentUserService.SetCurrentUser(user);
                    this.logger.LogInformation("User {Username} logged in successfully.", username);
                    return true;
                }

                this.logger.LogWarning("Login failed. Invalid password for user {Username}.", username);
                return false;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error during login for user {Username}", username);
                throw;
            }
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            this.logger.LogInformation("Attempting registration for user: {Username}", registerDto.Username);
            try
            {
                var existingUser = await this.userRepository.GetByUsernameAsync(registerDto.Username);
                if (existingUser != null)
                {
                    this.logger.LogWarning("Registration failed. User {Username} already exists.", registerDto.Username);
                    return false;
                }

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

                var newUser = new User
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    UserName = registerDto.Username,
                    Email = registerDto.Email,
                    PasswordHash = passwordHash,
                    CreatedAt = System.DateTime.UtcNow,
                    Role = "user",
                };

                await this.userRepository.AddAsync(newUser);
                this.logger.LogInformation("User {Username} registered successfully.", registerDto.Username);

                return true;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error during registration for user {Username}", registerDto.Username);
                throw;
            }
        }
    }
}