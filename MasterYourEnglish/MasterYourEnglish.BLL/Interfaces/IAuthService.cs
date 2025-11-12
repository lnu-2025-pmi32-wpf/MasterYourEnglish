using MasterYourEnglish.BLL.Models.DTOs;
using System.Threading.Tasks;

namespace MasterYourEnglish.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(string username, string password);
        Task<bool> RegisterAsync(RegisterDto registerDto);
    }
}