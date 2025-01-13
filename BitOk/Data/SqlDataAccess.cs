using Dapper;
using BitOk.Data.Utils;
using System.Data.SqlClient;

namespace BitOk.Data
{
    public class SqlDataAccess : ISqlDataAccess
    {
        private readonly IConfiguration _config;
        private readonly SqlConnection _connection;

        public SqlDataAccess(IConfiguration config)
        {
            _config = config;
            _connection = new SqlConnection(_config.GetConnectionString("Default"));

            SqlMapper.AddTypeHandler(new SqlDateOnlyTypeHandler());
            SqlMapper.AddTypeHandler(new SqlTimeOnlyTypeHandler());
        }

        public SqlConnection Connection => _connection;

        public async Task<List<T>> LoadData<T, U>(string sql, U parameters)
        {
            var data = await _connection.QueryAsync<T>(sql, parameters);
            return data.ToList();
        }

        public Task SaveData<T>(string sql, T parameters)
        {
            return _connection.ExecuteAsync(sql, parameters);
        }

        public Task ExecuteTransaction<T>(Dictionary<string, T> queries)
        {
            _connection.Open();

            var transaction = _connection.BeginTransaction();
            try
            {
                foreach (var query in queries)
                {
                    _connection.Execute(query.Key, query.Value, transaction);
                }

                transaction.Commit();
                transaction.Dispose();
            }
            catch (Exception)
            {
                transaction.Rollback();
                transaction.Dispose();
                throw;
            }

            return Task.CompletedTask;
        }
    }
}
