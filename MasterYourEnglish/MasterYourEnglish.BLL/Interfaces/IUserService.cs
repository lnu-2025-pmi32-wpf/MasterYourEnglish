namespace MasterYourEnglish.BLL.Interfaces
{
    using System.Threading.Tasks;
    using MasterYourEnglish.DAL.Entities;

    public interface IUserService
    {
        Task<User> GetUserByIdAsync(int userId);

        Task<bool> UpdateProfileAsync(int userId, string firstName, string lastName);
    }
}