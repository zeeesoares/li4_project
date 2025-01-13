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
    }
}
