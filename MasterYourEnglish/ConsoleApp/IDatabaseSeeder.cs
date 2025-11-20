using System.Threading.Tasks;

public interface IDatabaseSeeder
{

    Task ClearTables();
    Task PopulateDatabaseWithTestData();

}