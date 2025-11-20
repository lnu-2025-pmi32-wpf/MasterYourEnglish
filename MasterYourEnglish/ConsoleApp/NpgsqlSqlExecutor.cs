using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterYourEnglishApp
{
    public class NpgsqlSqlExecutor : ISqlExecutor
    {

        private const string ConnectionString = "Host=localhost;Port=5432;User id=postgres;Password=8888;Database=MasterYourEnglish";

        public async Task<int> ExecuteNonQueryAsync(string sql, IEnumerable<NpgsqlParameter> parameters)
        {
            await using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();
            await using var cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddRange(parameters.ToArray());
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> ExecuteScalarAsync(string sql, IEnumerable<NpgsqlParameter> parameters)
        {
            await using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();
            await using var cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddRange(parameters.ToArray());
            var result = await cmd.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : 0;
        }
    }
}