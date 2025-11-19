using MasterYourEnglish.DAL.Data;
using Microsoft.EntityFrameworkCore;

public abstract class DalTestBase : IDisposable
{
    protected ApplicationDbContext Context { get; }

    public DalTestBase()
    {
        
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        
        Context = new ApplicationDbContext(options);

        
        Context.Database.EnsureCreated();
    }

    
    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}