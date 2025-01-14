using BitOk.Data.Models;

namespace BitOk.Data.Services
{
    public interface IUserManager
    {
        Task CreateUser(string nome, string username, string passwordHash, string role);
        Task<UserModel?> GetUser(string username);
        Task<UserModel?> GetClient(string username);
        Task<UserModel?> GetAdmin(string username);
    }
}
