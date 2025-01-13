using BitOk.Data.Models;

namespace BitOk.Data.Services
{
    public interface IUserManager
    {
        public Task CreateUser(string nome, string username, string passwordHash, string role);

    }
}
