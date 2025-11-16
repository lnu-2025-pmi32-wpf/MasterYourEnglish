namespace MasterYourEnglish.DAL.Interfaces
{
    using System.Threading.Tasks;
    using MasterYourEnglish.DAL.Entities;

    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByUsernameAsync(string username);
    }
}