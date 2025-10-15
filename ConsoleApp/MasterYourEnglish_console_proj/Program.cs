using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MasterYourEnglishApp
{
    class Program
    {
        private const string connectionString = "Host=localhost;Port=5432;User id=postgres;Password=1234;Database=MasterYourEnglish";
        private static readonly Random random = new Random();

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("--- Додаток для роботи з БД MasterYourEnglish ---");

            try
            {
                await ClearTables();
                await PopulateDatabaseWithTestData();
                await ReadAndDisplayData();
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

        public static async Task PopulateDatabaseWithTestData()
        {
            Console.WriteLine("\nЗаповнення бази даних тестовими даними...");

            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            var firstNames = new[] { "Іван", "Марія", "Петро", "Олена", "Сергій", "Анна" };
            var lastNames = new[] { "Ковальчук", "Шевченко", "Бондаренко", "Ткаченко" };
            var topics = new[] { "Подорожі", "Їжа", "Технології", "Спорт", "Бізнес" };
            var words = new[] { "Apple", "Table", "Journey", "Code", "Success", "Recipe" };
            var levels = new[] { "A1", "A2", "B1", "B2", "C1", "C2" };

            var userIds = new List<int>();
            var topicIds = new List<int>();
            var flashcardIds = new List<int>();

            for (int i = 0; i < 30; i++)
            {
                var firstName = firstNames[random.Next(firstNames.Length)];
                var lastName = lastNames[random.Next(lastNames.Length)];
                var userName = $"{firstName.ToLower()}{lastName.ToLower()}{i}";

                var sql = "INSERT INTO users (first_name, last_name, user_name, email, password_hash, role) VALUES (@p1, @p2, @p3, @p4, @p5, @p6) RETURNING user_id;";
                await using var cmd = new NpgsqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("p1", firstName);
                cmd.Parameters.AddWithValue("p2", lastName);
                cmd.Parameters.AddWithValue("p3", userName);
                cmd.Parameters.AddWithValue("p4", $"{userName}@example.com");
                cmd.Parameters.AddWithValue("p5", "hashed_password_placeholder");
                cmd.Parameters.AddWithValue("p6", i % 10 == 0 ? "admin" : "user");

                userIds.Add((int)await cmd.ExecuteScalarAsync());
            }
            Console.WriteLine("Створено 30 користувачів.");

            foreach (var topicName in topics)
            {
                var sql = "INSERT INTO topics (name, description) VALUES (@p1, @p2) RETURNING topic_id;";
                await using var cmd = new NpgsqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("p1", topicName);
                cmd.Parameters.AddWithValue("p2", $"Опис для теми '{topicName}'");
                topicIds.Add((int)await cmd.ExecuteScalarAsync());
            }
            Console.WriteLine($"Створено {topics.Length} тем.");

            for (int i = 0; i < 50; i++)
            {
                var word = $"{words[random.Next(words.Length)]} {Guid.NewGuid().ToString().Substring(0, 4)}";

                var sql = @"
                    INSERT INTO flashcards (word, difficulty_level, meaning, topic_id, created_by, is_user_created)
                    VALUES (@p1, @p2, @p3, @p4, @p5, @p6) RETURNING flashcard_id;";
                await using var cmd = new NpgsqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("p1", word);
                cmd.Parameters.AddWithValue("p2", levels[random.Next(levels.Length)]);
                cmd.Parameters.AddWithValue("p3", "Значення слова...");
                cmd.Parameters.AddWithValue("p4", topicIds[random.Next(topicIds.Count)]);
                cmd.Parameters.AddWithValue("p5", userIds[random.Next(userIds.Count)]);
                cmd.Parameters.AddWithValue("p6", true);

                flashcardIds.Add((int)await cmd.ExecuteScalarAsync());
            }
            Console.WriteLine("Створено 50 флеш-карток.");
            Console.WriteLine("База даних успішно заповнена!");
        }

        public static async Task ReadAndDisplayData()
        {
            Console.WriteLine("\n--- Дані з бази даних ---");
            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            Console.WriteLine("\nУсі користувачі:");
            var sqlUsers = "SELECT user_name, email, role FROM users ORDER BY created_at DESC;";
            await using (var cmd = new NpgsqlCommand(sqlUsers, connection))
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                int count = 0;
                while (await reader.ReadAsync())
                {
                    count++;
                    Console.WriteLine($"{count,2}. {reader["user_name"],-25} ({reader["email"],-30}) роль: {reader["role"]}");
                }
            }

            Console.WriteLine("\nТеми:");
            var sqlTopics = "SELECT name FROM topics;";
            await using (var cmd = new NpgsqlCommand(sqlTopics, connection))
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                int count = 0;
                while (await reader.ReadAsync())
                {
                    count++;
                    Console.WriteLine($"{count,2}. {reader["name"]}");
                }
            }

            Console.WriteLine("\nУсі флеш-картки:");
            var sqlFlashcards = @"
                SELECT f.word, f.difficulty_level, t.name AS topic_name, u.user_name AS author
                FROM flashcards f
                JOIN topics t ON f.topic_id = t.topic_id
                JOIN users u ON f.created_by = u.user_id
                ORDER BY f.flashcard_id;";
            await using (var cmd = new NpgsqlCommand(sqlFlashcards, connection))
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                int count = 0;
                while (await reader.ReadAsync())
                {
                    count++;
                    Console.WriteLine($"{count,2}. Слово: '{reader["word"]}' ({reader["difficulty_level"]}), Тема: {reader["topic_name"]}, Автор: {reader["author"]}");
                }
            }
        }

        public static async Task ClearTables()
        {
            Console.WriteLine("\nОчищення таблиць...");
            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            var sql = "TRUNCATE TABLE users, topics, flashcards, flashcards_bundles, tests, questions RESTART IDENTITY CASCADE;";
            await using var cmd = new NpgsqlCommand(sql, connection);
            await cmd.ExecuteNonQueryAsync();
            Console.WriteLine("Таблиці очищено.");
        }
    }
}
