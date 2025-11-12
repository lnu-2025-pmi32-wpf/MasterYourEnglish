using MasterYourEnglish.DAL.Entities;
using System.Threading.Tasks;

namespace MasterYourEnglish.DAL.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByUsernameAsync(string username);
    }
}