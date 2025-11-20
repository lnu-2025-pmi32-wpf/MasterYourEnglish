using System;
using System.Threading.Tasks;

namespace MasterYourEnglishApp
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var seeder = new DatabaseSeeder();

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("--- Додаток для роботи з БД MasterYourEnglish ---");

            try
            {
                await seeder.ClearTables();
                await seeder.PopulateDatabaseWithTestData();

                await seeder.DisplayAllTablesData();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nСталася помилка: {ex.Message}");
                Console.WriteLine("\nПеревірте, чи правильно вказано пароль у 'connectionString' та чи запущено сервер PostgreSQL.");
                Console.ResetColor();
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу для виходу.");
            Console.ReadKey();
        }
    }
}