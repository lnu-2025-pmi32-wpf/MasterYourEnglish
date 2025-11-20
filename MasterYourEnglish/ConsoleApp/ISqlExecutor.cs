using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ISqlExecutor
{
    Task<int> ExecuteNonQueryAsync(string sql, IEnumerable<NpgsqlParameter> parameters);

    Task<int> ExecuteScalarAsync(string sql, IEnumerable<NpgsqlParameter> parameters);
}