using MasterYourEnglish.DAL.Entities;
using System.Threading.Tasks;

namespace MasterYourEnglish.BLL.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(int userId);
        Task<bool> UpdateProfileAsync(int userId, string firstName, string lastName);
    }
}