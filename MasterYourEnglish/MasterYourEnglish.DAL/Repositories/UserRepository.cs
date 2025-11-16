namespace MasterYourEnglish.DAL.Repositories
{
    using System.Threading.Tasks;
    using MasterYourEnglish.DAL.Data;
    using MasterYourEnglish.DAL.Entities;
    using MasterYourEnglish.DAL.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await this.dbSet.FirstOrDefaultAsync(u => u.UserName == username);
        }
    }
}