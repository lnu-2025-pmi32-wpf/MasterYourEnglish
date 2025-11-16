namespace MasterYourEnglish.BLL.Services
{
    using System.Threading.Tasks;
    using MasterYourEnglish.BLL.Interfaces;
    using MasterYourEnglish.BLL.Models.DTOs;
    using MasterYourEnglish.DAL.Entities;
    using MasterYourEnglish.DAL.Interfaces;

    public class AuthService : IAuthService
    {
        private readonly IUserRepository userRepository;
        private readonly ICurrentUserService currentUserService;

        public AuthService(IUserRepository userRepository, ICurrentUserService currentUserService)
        {
            this.userRepository = userRepository;
            this.currentUserService = currentUserService;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var user = await this.userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                return false;
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            if (isPasswordValid)
            {
                this.currentUserService.SetCurrentUser(user);
                return true;
            }

            return false;
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await this.userRepository.GetByUsernameAsync(registerDto.Username);
            if (existingUser != null)
            {
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

            return true;
        }
    }
}