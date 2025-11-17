namespace MasterYourEnglish.BLL.Interfaces
{
    using System.Threading.Tasks;
    using MasterYourEnglish.BLL.Models.DTOs;

    public interface IAuthService
    {
        Task<bool> LoginAsync(string username, string password);

        Task<bool> RegisterAsync(RegisterDto registerDto);
    }
}