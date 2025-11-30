using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MasterYourEnglishApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var seeder = new DatabaseSeeder();

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Log.Information("--- Додаток для роботи з БД MasterYourEnglish ---");

            try
            {
                Log.Information("Clearing tables...");
                await seeder.ClearTables();
                Log.Information("Populating database with test data...");
                await seeder.PopulateDatabaseWithTestData();
                Log.Information("Displaying all tables data...");
                await seeder.DisplayAllTablesData();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Сталася помилка: {Message}", ex.Message);
                Log.Error("Перевірте, чи правильно вказано пароль у 'connectionString' та чи запущено сервер PostgreSQL.");
            }
            finally
            {
                Log.Information("Натисніть будь-яку клавішу для виходу.");
                Console.ReadKey();
            }
        }
    }
}