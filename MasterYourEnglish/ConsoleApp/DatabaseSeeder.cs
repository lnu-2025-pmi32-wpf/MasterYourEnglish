using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterYourEnglishApp
{
    public class DatabaseSeeder
    {
        private static readonly Random Random = new Random();
        private readonly ISqlExecutor _sqlExecutor;

        public DatabaseSeeder(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor;
        }

        public DatabaseSeeder() : this(new NpgsqlSqlExecutor())
        {
        }

        public async Task ClearTables()
        {
            Console.WriteLine("\nОчищення таблиць...");
            var sql = "TRUNCATE TABLE users, topics, flashcards, flashcards_bundles, tests, questions RESTART IDENTITY CASCADE;";
            await _sqlExecutor.ExecuteNonQueryAsync(sql, new List<NpgsqlParameter>());
            Console.WriteLine("Таблиці успішно очищено.");
        }

        public async Task PopulateDatabaseWithTestData()
        {
            Console.WriteLine("\nЗаповнення бази даних тестовими даними...");

            var userIds = new List<int>();
            var topicIds = new List<int>();
            var flashcardIds = new List<int>();
            var flashcardBundleIds = new List<int>();
            var questionDetails = new Dictionary<int, string>();
            var testIds = new List<int>();


            var topics = new[] { "Подорожі", "Їжа", "Технології", "Спорт", "Бізнес", "Наука", "Мистецтво" };
            var words = new[] { "Apple", "Table", "Journey", "Code", "Success", "Recipe", "Galaxy", "Velocity" };
            var levels = new[] { "A1", "A2", "B1", "B2", "C1", "C2" };
            var questionTypes = new[] { "single_choice", "multiple_choice", "text_input" };
            var firstNames = new[] { "Іван", "Марія", "Петро", "Олена", "Сергій", "Анна" };
            var lastNames = new[] { "Ковальчук", "Шевченко", "Бондаренко", "Ткаченко", "Мельник", "Поліщук" };



            for (int i = 0; i < 30; i++)
            {
                var firstName = firstNames[Random.Next(firstNames.Length)];
                var lastName = lastNames[Random.Next(lastNames.Length)];
                var userName = $"{firstName.ToLower()}{i}{lastName.ToLower()}";
                var sql = "INSERT INTO users (first_name, last_name, user_name, email, password_hash, role) VALUES (@p1, @p2, @p3, @p4, @p5, @p6) RETURNING user_id;";
                var parameters = new NpgsqlParameter[] {
                    new("p1", firstName), new("p2", lastName), new("p3", userName),
                    new("p4", $"{userName}@example.com"), new("p5", "hashed_password_placeholder"),
                    new("p6", i % 10 == 0 ? "admin" : "user")
                };
                userIds.Add(await _sqlExecutor.ExecuteScalarAsync(sql, parameters));
            }
            Console.WriteLine("Створено 30 користувачів.");


            foreach (var topicName in topics)
            {
                var sql = "INSERT INTO topics (name, description) VALUES (@p1, @p2) RETURNING topic_id;";
                var parameters = new NpgsqlParameter[] { new("p1", topicName), new("p2", $"Опис для теми '{topicName}'") };
                topicIds.Add(await _sqlExecutor.ExecuteScalarAsync(sql, parameters));
            }
            Console.WriteLine($"Створено {topics.Length} тем.");


            for (int i = 0; i < 100; i++)
            {
                var sql = "INSERT INTO flashcards (word, difficulty_level, transcription, meaning, part_of_speech, example, topic_id, created_by, is_user_created) VALUES (@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9) RETURNING flashcard_id;";
                var word = $"{words[Random.Next(words.Length)]} {Guid.NewGuid().ToString().Substring(0, 4)}";

                var parameters = new NpgsqlParameter[] {
                    new("p1", word), new("p2", levels[Random.Next(levels.Length)]),
                    new("p3", $"/trænsˈkrɪpʃən {i}/"), new("p4", $"Значення слова '{word}'"),
                    new("p5", "noun"), new("p6", $"Example sentence for {word}."),
                    new("p7", topicIds[Random.Next(topicIds.Count)]), new("p8", userIds[Random.Next(userIds.Count)]),
                    new("p9", Random.Next(2) == 0)
                };

                flashcardIds.Add(await _sqlExecutor.ExecuteScalarAsync(sql, parameters));
            }
            Console.WriteLine("Створено 100 флеш-карток.");


            for (int i = 0; i < 20; i++)
            {
                var sql = "INSERT INTO flashcards_bundles (title, description, difficulty_level, topic_id, created_by, is_published, is_user_created) VALUES (@p1, @p2, @p3, @p4, @p5, @p6, @p7) RETURNING flashcards_bundle_id;";
                var parameters = new NpgsqlParameter[] {
                    new("p1", $"Набір карток #{i+1}"), new("p2", $"Опис для набору #{i+1}"),
                    new("p3", levels[Random.Next(levels.Length)]), new("p4", topicIds[Random.Next(topicIds.Count)]),
                    new("p5", userIds[Random.Next(userIds.Count)]), new("p6", Random.Next(2) == 0),
                    new("p7", Random.Next(2) == 0)
                };
                flashcardBundleIds.Add(await _sqlExecutor.ExecuteScalarAsync(sql, parameters));
            }
            Console.WriteLine("Створено 20 наборів флеш-карток.");


            foreach (var bundleId in flashcardBundleIds)
            {
                var cardsInBundle = flashcardIds.OrderBy(x => Random.Next()).Take(Random.Next(5, 20)).ToList();
                foreach (var cardId in cardsInBundle)
                {
                    var sql = "INSERT INTO flashcards_bundle_items (flashcards_bundle_id, flashcard_id) VALUES (@p1, @p2);";
                    var parameters = new NpgsqlParameter[] { new("p1", bundleId), new("p2", cardId) };
                    await _sqlExecutor.ExecuteNonQueryAsync(sql, parameters);
                }
                var updateSql = "UPDATE flashcards_bundles SET total_flashcards_count = @p1 WHERE flashcards_bundle_id = @p2;";
                var updateParams = new NpgsqlParameter[] { new("p1", cardsInBundle.Count), new("p2", bundleId) };
                await _sqlExecutor.ExecuteNonQueryAsync(updateSql, updateParams);
            }
            Console.WriteLine("Додано картки до наборів.");


            foreach (var userId in userIds)
            {
                var savedCards = flashcardIds.OrderBy(x => Random.Next()).Take(Random.Next(10, 30)).ToList();
                foreach (var cardId in savedCards)
                {
                    var sql = "INSERT INTO saved_flashcards (user_id, flashcard_id) VALUES (@p1, @p2) ON CONFLICT DO NOTHING;";
                    var parameters = new NpgsqlParameter[] { new("p1", userId), new("p2", cardId) };
                    await _sqlExecutor.ExecuteNonQueryAsync(sql, parameters);
                }
            }
            Console.WriteLine("Збережено картки для користувачів.");


            var questionIds = new List<int>();
            for (int i = 0; i < 200; i++)
            {
                var qType = questionTypes[Random.Next(questionTypes.Length)];
                var sql = "INSERT INTO questions (text, question_type, difficulty_level, created_by, topic_id) VALUES (@p1, @p2, @p3, @p4, @p5) RETURNING question_id;";
                var parameters = new NpgsqlParameter[] {
                    new("p1", $"Текст запитання #{i+1}?"), new("p2", qType),
                    new("p3", levels[Random.Next(levels.Length)]), new("p4", userIds[Random.Next(userIds.Count)]),
                    new("p5", topicIds[Random.Next(topicIds.Count)])
                };
                var qId = await _sqlExecutor.ExecuteScalarAsync(sql, parameters);
                questionIds.Add(qId);
                questionDetails.Add(qId, qType);

                if (qType == "single_choice" || qType == "multiple_choice")
                {
                    var correctOptionIndex = (qType == "single_choice") ? Random.Next(4) : -1;
                    for (int j = 0; j < 4; j++)
                    {
                        bool isCorrect = (qType == "single_choice") ? (j == correctOptionIndex) : (Random.Next(2) == 0);
                        var optSql = "INSERT INTO question_options (question_id, text, is_correct) VALUES (@p1, @p2, @p3);";
                        var optParams = new NpgsqlParameter[] {
                            new("p1", qId), new("p2", $"Варіант відповіді {j+1}"), new("p3", isCorrect)
                        };
                        await _sqlExecutor.ExecuteNonQueryAsync(optSql, optParams);
                    }
                }
            }
            Console.WriteLine("Створено 200 запитань та варіантів відповідей.");


            for (int i = 0; i < 15; i++)
            {
                var sql = "INSERT INTO tests (title, description, topic_id, difficulty_level, created_by, is_published, is_user_created) VALUES (@p1, @p2, @p3, @p4, @p5, @p6, @p7) RETURNING test_id;";
                var parameters = new NpgsqlParameter[] {
                    new("p1", $"Тест #{i+1}"), new("p2", $"Опис для тесту #{i+1}"),
                    new("p3", topicIds[Random.Next(topicIds.Count)]), new("p4", levels[Random.Next(levels.Length)]),
                    new("p5", userIds[Random.Next(userIds.Count)]), new("p6", Random.Next(2) == 0),
                    new("p7", Random.Next(2) == 0)
                };
                testIds.Add(await _sqlExecutor.ExecuteScalarAsync(sql, parameters));
            }
            Console.WriteLine("Створено 15 тестів.");


            foreach (var testId in testIds)
            {
                var questionsInTest = questionIds.OrderBy(x => Random.Next()).Take(Random.Next(10, 25)).ToList();
                foreach (var qId in questionsInTest)
                {
                    var sql = "INSERT INTO test_questions (test_id, question_id) VALUES (@p1, @p2);";
                    var parameters = new NpgsqlParameter[] { new("p1", testId), new("p2", qId) };
                    await _sqlExecutor.ExecuteNonQueryAsync(sql, parameters);
                }
                var updateSql = "UPDATE tests SET total_questions_count = @p1 WHERE test_id = @p2;";
                var updateParams = new NpgsqlParameter[] { new("p1", questionsInTest.Count), new("p2", testId) };
                await _sqlExecutor.ExecuteNonQueryAsync(updateSql, updateParams);
            }
            Console.WriteLine("Додано запитання до тестів.");


            var flashcardAttempts = new List<int>();
            for (int i = 0; i < 50; i++)
            {
                var sql = "INSERT INTO flashcards_bundle_attempts (flashcards_bundle_id, user_id) VALUES (@p1, @p2) RETURNING attempt_id;";
                var parameters = new NpgsqlParameter[]{
                             new("p1", flashcardBundleIds[Random.Next(flashcardBundleIds.Count)]),
                             new("p2", userIds[Random.Next(userIds.Count)])
                         };
                flashcardAttempts.Add(await _sqlExecutor.ExecuteScalarAsync(sql, parameters));
            }

            foreach (var attemptId in flashcardAttempts)
            {
                var ansSql = "INSERT INTO flashcards_attempts_answers (attempt_id, flashcard_id, is_known) VALUES (@p1, @p2, @p3);";
                var ansParams = new NpgsqlParameter[]{
                           new("p1", attemptId),
                           new("p2", flashcardIds[Random.Next(flashcardIds.Count)]),
                           new("p3", Random.Next(2)==0)
                         };
                await _sqlExecutor.ExecuteNonQueryAsync(ansSql, ansParams);
            }
            Console.WriteLine("Створено спроби проходження наборів карток.");

            var testAttempts = new List<int>();
            for (int i = 0; i < 70; i++)
            {
                var sql = "INSERT INTO test_attempts (test_id, user_id) VALUES (@p1, @p2) RETURNING attempt_id;";
                var parameters = new NpgsqlParameter[]{
                             new("p1", testIds[Random.Next(testIds.Count)]),
                             new("p2", userIds[Random.Next(userIds.Count)])
                         };
                testAttempts.Add(await _sqlExecutor.ExecuteScalarAsync(sql, parameters));
            }

            foreach (var attemptId in testAttempts)
            {
                bool isCorrect = Random.Next(10) > 3;
                var ansSql = "INSERT INTO test_attempts_answers (attempt_id, question_id, is_correct) VALUES (@p1, @p2, @p3);";
                var ansParams = new NpgsqlParameter[]{
                           new("p1", attemptId),
                           new("p2", questionIds[Random.Next(questionIds.Count)]),
                           new("p3", isCorrect)
                         };
                await _sqlExecutor.ExecuteNonQueryAsync(ansSql, ansParams);

                var updSql = "UPDATE test_attempts SET score = @p1, finished_at = CURRENT_TIMESTAMP WHERE attempt_id = @p2;";
                var updParams = new NpgsqlParameter[] { new("p1", Random.NextDouble() * 100), new("p2", attemptId) };
                await _sqlExecutor.ExecuteNonQueryAsync(updSql, updParams);
            }
            Console.WriteLine("Створено спроби проходження тестів та їхні результати.");

            Console.WriteLine("\nБаза даних успішно заповнена!");
        }

        public async Task DisplayAllTablesData()
        {
            // Цей метод вимагає прямого ADO.NET коду для читання даних.
        }
    }
}