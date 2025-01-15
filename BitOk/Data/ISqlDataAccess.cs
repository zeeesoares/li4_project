using System.Data.SqlClient;

namespace BitOk.Data
{
    public interface ISqlDataAccess
    {
        SqlConnection Connection { get; }

        Task<T> ExecuteScalarAsync<T>(string sql, object parameters = null);
        Task<List<T>> LoadData<T, U>(string sql, U parameters);

        Task SaveData<T>(string sql, T parameters);

        Task ExecuteTransaction<T>(Dictionary<string, T> queries);
    }
}