using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterYourEnglishApp
{
    class Program
    {
        private const string connectionString = "Host=localhost;Port=5432;User id=postgres;Password=8888;Database=MasterYourEnglish";
        private static readonly Random random = new Random();

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("--- Додаток для роботи з БД MasterYourEnglish ---");

            try
            {
                await ClearTables();
                await PopulateDatabaseWithTestData();

                await DisplayAllTablesData();
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

        public static async Task DisplayAllTablesData()
        {
            Console.WriteLine("\n--- Починаємо вивід даних з усіх таблиць (перші 10 рядків) ---");

            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            var tableNames = new[] {
                "users", "topics", "flashcards", "flashcards_bundles",
                "flashcards_bundle_items", "saved_flashcards", "flashcards_bundle_attempts",
                "flashcards_attempts_answers", "tests", "questions", "question_options",
                "test_questions", "test_attempts", "test_attempts_answers",
                "test_attempts_answers_selected_options"
            };

            foreach (var tableName in tableNames)
            {
                Console.WriteLine($"\n--- Таблиця: {tableName} ---");
                await DisplayQueryResultsAsync(connection, $"SELECT * FROM {tableName} LIMIT 10;");
            }
        }

        
        private static async Task DisplayQueryResultsAsync(NpgsqlConnection connection, string sqlQuery)
        {
            try
            {
                await using var cmd = new NpgsqlCommand(sqlQuery, connection);
                await using var reader = await cmd.ExecuteReaderAsync();

                if (!reader.HasRows)
                {
                    Console.WriteLine("Таблиця порожня або не містить даних.");
                    return;
                }

                var columnNames = new List<string>();
                var columnWidths = new List<int>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var name = reader.GetName(i);
                    columnNames.Add(name);
                    columnWidths.Add(name.Length); 
                }

                var rows = new List<List<string>>();
                while (await reader.ReadAsync())
                {
                    var row = new List<string>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var value = reader.IsDBNull(i) ? "NULL" : reader[i].ToString();
                        row.Add(value);
                        if (value.Length > columnWidths[i])
                        {
                            columnWidths[i] = value.Length;
                        }
                    }
                    rows.Add(row);
                }

                await reader.CloseAsync();

                string header = "|";
                for (int i = 0; i < columnNames.Count; i++)
                {
                    header += $" {columnNames[i].PadRight(columnWidths[i])} |";
                }
                Console.WriteLine(header);

                string separator = "+";
                for (int i = 0; i < columnNames.Count; i++)
                {
                    separator += new string('-', columnWidths[i] + 2) + "+";
                }
                Console.WriteLine(separator);

                foreach (var row in rows)
                {
                    string rowString = "|";
                    for (int i = 0; i < row.Count; i++)
                    {
                        rowString += $" {row[i].PadRight(columnWidths[i])} |";
                    }
                    Console.WriteLine(rowString);
                }

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Не вдалося виконати запит '{sqlQuery}'. Помилка: {ex.Message}");
                Console.ResetColor();
            }
        }

        public static async Task PopulateDatabaseWithTestData()
        {
            Console.WriteLine("\nЗаповнення бази даних тестовими даними...");

            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var userIds = new List<int>();
                var topicIds = new List<int>();
                var flashcardIds = new List<int>();
                var flashcardBundleIds = new List<int>();
                var questionDetails = new Dictionary<int, string>();
                var testIds = new List<int>();

                var firstNames = new[] { "Іван", "Марія", "Петро", "Олена", "Сергій", "Анна" };
                var lastNames = new[] { "Ковальчук", "Шевченко", "Бондаренко", "Ткаченко", "Мельник", "Поліщук" };
                var topics = new[] { "Подорожі", "Їжа", "Технології", "Спорт", "Бізнес", "Наука", "Мистецтво" };
                var words = new[] { "Apple", "Table", "Journey", "Code", "Success", "Recipe", "Galaxy", "Velocity" };
                var levels = new[] { "A1", "A2", "B1", "B2", "C1", "C2" };
                var questionTypes = new[] { "single_choice", "multiple_choice", "text_input" };

                // 1. Users
                for (int i = 0; i < 30; i++)
                {
                    var firstName = firstNames[random.Next(firstNames.Length)];
                    var lastName = lastNames[random.Next(lastNames.Length)];
                    var userName = $"{firstName.ToLower()}{i}{lastName.ToLower()}";
                    await using var cmd = new NpgsqlCommand("INSERT INTO users (first_name, last_name, user_name, email, password_hash, role) VALUES (@p1, @p2, @p3, @p4, @p5, @p6) RETURNING user_id;", connection);
                    cmd.Parameters.AddRange(new NpgsqlParameter[] {
                        new("p1", firstName), new("p2", lastName), new("p3", userName),
                        new("p4", $"{userName}@example.com"), new("p5", "hashed_password_placeholder"),
                        new("p6", i % 10 == 0 ? "admin" : "user")
                    });
                    userIds.Add((int)await cmd.ExecuteScalarAsync());
                }
                Console.WriteLine("Створено 30 користувачів.");

                // 2. Topics
                foreach (var topicName in topics)
                {
                    await using var cmd = new NpgsqlCommand("INSERT INTO topics (name, description) VALUES (@p1, @p2) RETURNING topic_id;", connection);
                    cmd.Parameters.AddRange(new NpgsqlParameter[] { new("p1", topicName), new("p2", $"Опис для теми '{topicName}'") });
                    topicIds.Add((int)await cmd.ExecuteScalarAsync());
                }
                Console.WriteLine($"Створено {topics.Length} тем.");

                // 3. Flashcards
                for (int i = 0; i < 100; i++)
                {
                    await using var cmd = new NpgsqlCommand("INSERT INTO flashcards (word, difficulty_level, transcription, meaning, part_of_speech, example, topic_id, created_by, is_user_created) VALUES (@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9) RETURNING flashcard_id;", connection);
                    var word = $"{words[random.Next(words.Length)]} {Guid.NewGuid().ToString().Substring(0, 4)}";
                    cmd.Parameters.AddRange(new NpgsqlParameter[] {
                        new("p1", word), new("p2", levels[random.Next(levels.Length)]),
                        new("p3", $"/trænsˈkrɪpʃən {i}/"), new("p4", $"Значення слова '{word}'"),
                        new("p5", "noun"), new("p6", $"Example sentence for {word}."),
                        new("p7", topicIds[random.Next(topicIds.Count)]), new("p8", userIds[random.Next(userIds.Count)]),
                        new("p9", random.Next(2) == 0)
                    });
                    flashcardIds.Add((int)await cmd.ExecuteScalarAsync());
                }
                Console.WriteLine("Створено 100 флеш-карток.");

                // 4. Flashcards_Bundles
                for (int i = 0; i < 20; i++)
                {
                    await using var cmd = new NpgsqlCommand("INSERT INTO flashcards_bundles (title, description, difficulty_level, topic_id, created_by, is_published, is_user_created) VALUES (@p1, @p2, @p3, @p4, @p5, @p6, @p7) RETURNING flashcards_bundle_id;", connection);
                    cmd.Parameters.AddRange(new NpgsqlParameter[] {
                        new("p1", $"Набір карток #{i+1}"), new("p2", $"Опис для набору #{i+1}"),
                        new("p3", levels[random.Next(levels.Length)]), new("p4", topicIds[random.Next(topicIds.Count)]),
                        new("p5", userIds[random.Next(userIds.Count)]), new("p6", random.Next(2) == 0),
                        new("p7", random.Next(2) == 0)
                    });
                    flashcardBundleIds.Add((int)await cmd.ExecuteScalarAsync());
                }
                Console.WriteLine("Створено 20 наборів флеш-карток.");

                // 5. Flashcards_Bundle_Items
                foreach (var bundleId in flashcardBundleIds)
                {
                    var cardsInBundle = flashcardIds.OrderBy(x => random.Next()).Take(random.Next(5, 20)).ToList();
                    foreach (var cardId in cardsInBundle)
                    {
                        await using var cmd = new NpgsqlCommand("INSERT INTO flashcards_bundle_items (flashcards_bundle_id, flashcard_id) VALUES (@p1, @p2);", connection);
                        cmd.Parameters.AddRange(new NpgsqlParameter[] { new("p1", bundleId), new("p2", cardId) });
                        await cmd.ExecuteNonQueryAsync();
                    }
                    await using var updateCmd = new NpgsqlCommand("UPDATE flashcards_bundles SET total_flashcards_count = @p1 WHERE flashcards_bundle_id = @p2;", connection);
                    updateCmd.Parameters.AddRange(new NpgsqlParameter[] { new("p1", cardsInBundle.Count), new("p2", bundleId) });
                    await updateCmd.ExecuteNonQueryAsync();
                }
                Console.WriteLine("Додано картки до наборів.");

                // 6. Saved_Flashcards
                foreach (var userId in userIds)
                {
                    var savedCards = flashcardIds.OrderBy(x => random.Next()).Take(random.Next(10, 30)).ToList();
                    foreach (var cardId in savedCards)
                    {
                        await using var cmd = new NpgsqlCommand("INSERT INTO saved_flashcards (user_id, flashcard_id) VALUES (@p1, @p2) ON CONFLICT DO NOTHING;", connection);
                        cmd.Parameters.AddRange(new NpgsqlParameter[] { new("p1", userId), new("p2", cardId) });
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                Console.WriteLine("Збережено картки для користувачів.");

                // 7. Questions and Question_Options
                var questionIds = new List<int>();
                for (int i = 0; i < 200; i++)
                {
                    var qType = questionTypes[random.Next(questionTypes.Length)];
                    await using var cmd = new NpgsqlCommand("INSERT INTO questions (text, question_type, difficulty_level, created_by, topic_id) VALUES (@p1, @p2, @p3, @p4, @p5) RETURNING question_id;", connection);
                    cmd.Parameters.AddRange(new NpgsqlParameter[] {
                        new("p1", $"Текст запитання #{i+1}?"), new("p2", qType),
                        new("p3", levels[random.Next(levels.Length)]), new("p4", userIds[random.Next(userIds.Count)]),
                        new("p5", topicIds[random.Next(topicIds.Count)])
                    });
                    var qId = (int)await cmd.ExecuteScalarAsync();
                    questionIds.Add(qId);
                    questionDetails.Add(qId, qType);

                    if (qType == "single_choice" || qType == "multiple_choice")
                    {
                        var correctOptionIndex = (qType == "single_choice") ? random.Next(4) : -1;
                        for (int j = 0; j < 4; j++)
                        {
                            bool isCorrect = (qType == "single_choice") ? (j == correctOptionIndex) : (random.Next(2) == 0);
                            await using var optCmd = new NpgsqlCommand("INSERT INTO question_options (question_id, text, is_correct) VALUES (@p1, @p2, @p3);", connection);
                            optCmd.Parameters.AddRange(new NpgsqlParameter[] {
                                new("p1", qId), new("p2", $"Варіант відповіді {j+1}"), new("p3", isCorrect)
                            });
                            await optCmd.ExecuteNonQueryAsync();
                        }
                    }
                }
                Console.WriteLine("Створено 200 запитань та варіантів відповідей.");

                // 8. Tests
                for (int i = 0; i < 15; i++)
                {
                    await using var cmd = new NpgsqlCommand("INSERT INTO tests (title, description, topic_id, difficulty_level, created_by, is_published, is_user_created) VALUES (@p1, @p2, @p3, @p4, @p5, @p6, @p7) RETURNING test_id;", connection);
                    cmd.Parameters.AddRange(new NpgsqlParameter[] {
                        new("p1", $"Тест #{i+1}"), new("p2", $"Опис для тесту #{i+1}"),
                        new("p3", topicIds[random.Next(topicIds.Count)]), new("p4", levels[random.Next(levels.Length)]),
                        new("p5", userIds[random.Next(userIds.Count)]), new("p6", random.Next(2) == 0),
                        new("p7", random.Next(2) == 0)
                    });
                    testIds.Add((int)await cmd.ExecuteScalarAsync());
                }
                Console.WriteLine("Створено 15 тестів.");

                // 9. Test_Questions and update count
                foreach (var testId in testIds)
                {
                    var questionsInTest = questionIds.OrderBy(x => random.Next()).Take(random.Next(10, 25)).ToList();
                    foreach (var qId in questionsInTest)
                    {
                        await using var cmd = new NpgsqlCommand("INSERT INTO test_questions (test_id, question_id) VALUES (@p1, @p2);", connection);
                        cmd.Parameters.AddRange(new NpgsqlParameter[] { new("p1", testId), new("p2", qId) });
                        await cmd.ExecuteNonQueryAsync();
                    }
                    await using var updateCmd = new NpgsqlCommand("UPDATE tests SET total_questions_count = @p1 WHERE test_id = @p2;", connection);
                    updateCmd.Parameters.AddRange(new NpgsqlParameter[] { new("p1", questionsInTest.Count), new("p2", testId) });
                    await updateCmd.ExecuteNonQueryAsync();
                }
                Console.WriteLine("Додано запитання до тестів.");

                // 10. All Attempts and Answers (Flashcards & Tests)
                var flashcardAttempts = new List<int>();
                for (int i = 0; i < 50; i++)
                {
                    await using var cmd = new NpgsqlCommand("INSERT INTO flashcards_bundle_attempts (flashcards_bundle_id, user_id) VALUES (@p1, @p2) RETURNING attempt_id;", connection);
                    cmd.Parameters.AddRange(new NpgsqlParameter[]{
                             new("p1", flashcardBundleIds[random.Next(flashcardBundleIds.Count)]),
                             new("p2", userIds[random.Next(userIds.Count)])
                          });
                    flashcardAttempts.Add((int)await cmd.ExecuteScalarAsync());
                }

                foreach (var attemptId in flashcardAttempts)
                {
                    await using var ansCmd = new NpgsqlCommand("INSERT INTO flashcards_attempts_answers (attempt_id, flashcard_id, is_known) VALUES (@p1, @p2, @p3);", connection);
                    ansCmd.Parameters.AddRange(new NpgsqlParameter[]{
                           new("p1", attemptId),
                           new("p2", flashcardIds[random.Next(flashcardIds.Count)]),
                           new("p3", random.Next(2)==0)
                        });
                    await ansCmd.ExecuteNonQueryAsync();
                }
                Console.WriteLine("Створено спроби проходження наборів карток.");

                var testAttempts = new List<int>();
                for (int i = 0; i < 70; i++)
                {
                    await using var cmd = new NpgsqlCommand("INSERT INTO test_attempts (test_id, user_id) VALUES (@p1, @p2) RETURNING attempt_id;", connection);
                    cmd.Parameters.AddRange(new NpgsqlParameter[]{
                             new("p1", testIds[random.Next(testIds.Count)]),
                             new("p2", userIds[random.Next(userIds.Count)])
                          });
                    testAttempts.Add((int)await cmd.ExecuteScalarAsync());
                }

                foreach (var attemptId in testAttempts)
                {
                    bool isCorrect = random.Next(10) > 3;
                    await using var ansCmd = new NpgsqlCommand("INSERT INTO test_attempts_answers (attempt_id, question_id, is_correct) VALUES (@p1, @p2, @p3);", connection);
                    ansCmd.Parameters.AddRange(new NpgsqlParameter[]{
                           new("p1", attemptId),
                           new("p2", questionIds[random.Next(questionIds.Count)]),
                           new("p3", isCorrect)
                        });
                    await ansCmd.ExecuteNonQueryAsync();

                    await using var updCmd = new NpgsqlCommand("UPDATE test_attempts SET score = @p1, finished_at = CURRENT_TIMESTAMP WHERE attempt_id = @p2;", connection);
                    updCmd.Parameters.AddRange(new NpgsqlParameter[] { new("p1", random.NextDouble() * 100), new("p2", attemptId) });
                    await updCmd.ExecuteNonQueryAsync();
                }
                Console.WriteLine("Створено спроби проходження тестів та їхні результати.");


                await transaction.CommitAsync();
                Console.WriteLine("\nБаза даних успішно заповнена!");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                Console.WriteLine("Під час заповнення сталася помилка, транзакцію відкочено.");
                throw;
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
            Console.WriteLine("Таблиці успішно очищено.");
        }
    }
}