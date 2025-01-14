using BitOk.Data.Models;

namespace BitOk.Data.Services
{
    public class UserManager : IUserManager
    {
        private readonly ISqlDataAccess _db;

        public UserManager(ISqlDataAccess db)
        {
            _db = db;
        }
        public Task CreateUser(string nome, string username, string passwordHash, string role)
        {
            var queries = new Dictionary<string, dynamic>
             {
                {
            @"INSERT INTO dbo.Utilizador (Nome, Username, Password, Role) 
              VALUES (@Nome, @Username, @Password, @Role)",
            new { Nome = nome, Username = username, Password = passwordHash, Role = role }
                }
            };
            return _db.ExecuteTransaction(queries);
        }


        public Task<UserModel?> GetUser(string username)
        {
            const string sql = "SELECT * FROM dbo.Utilizador WHERE Username = @Username";
            return _db.LoadData<UserModel, dynamic>(sql, new { Username = username })
                .ContinueWith(task => task.Result.FirstOrDefault());
        }

        public Task<UserModel?> GetClient(string username)
        {
            const string sql = @"
            SELECT * FROM dbo.Utilizador AS u
            WHERE u.Username = @Username AND u.Role = 'client'";

            return _db.LoadData<UserModel, dynamic>(sql, new { Username = username })
                .ContinueWith(task => task.Result.FirstOrDefault());
        }

        public Task<UserModel?> GetAdmin(string username)
        {
            const string sql = @"
            SELECT * FROM dbo.Utilizador AS u
            WHERE u.Username = @Username AND u.Role = 'admin'";

            return _db.LoadData<UserModel, dynamic>(sql, new { Username = username })
                .ContinueWith(task => task.Result.FirstOrDefault());
        }

    }
}
